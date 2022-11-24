

connected = false
sending = false
response = "..."
responseError = false
qrCodePlayer = ""
dataBytes = ""

function onTick()
	--output.setBool(1, connected)
	--output.setBool(2, responseError)		
		
	if (sending) then
		return
	end	
	sending = true	
	
	if not connected then
		async.httpGet(18146, "/?startup=true")
		return
	end
	
	if qrCodePlayer == "" then
		async.httpGet(18146, "/qrcode?href=http://[ipAddress]:18146/controller?player=1")
		return
	end	

	-- Read 
	if string.len(dataBytes) > 0 then
		for i=1,32,1 do 
			local v = string.unpack("<d", dataBytes, i * 8 - 7)
			output.setNumber(i, v)
		end
		for i=1,32,1 do 
			local v = string.unpack("<b", dataBytes, 32 * 8 - 7 + i + 7)
			output.setBool(i, v==1)
		end
	end

	-- Write 
	local g = input.getNumber
	local b = getBoolAsByte
	local inputBytes = string.pack("<ddddddddddddddddddddddddddddddddbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb", 
	  	g(1),g(2),g(3),g(4),g(5),g(6),g(7),g(8),g(9),g(10),g(11),g(12),g(13),g(14),g(15),g(16),
	  	g(17),g(18),g(19),g(20),g(21),g(22),g(23),g(24),g(25),g(26),g(27),g(28),g(29),g(30),g(31),g(32),
  		b(1),b(2),b(3),b(4),b(5),b(6),b(7),b(8),b(9),b(10),b(11),b(12),b(13),b(14),b(15),b(16),
	  	b(17),b(18),b(19),b(20),b(21),b(22),b(23),b(24),b(2),b(26),b(27),b(28),b(29),b(30),b(31),b(32)	  
	  	)

	local inputBase64 = enc(inputBytes)
	async.httpGet(18146, "/controller?data=" .. inputBase64)
end


function getBoolAsByte(index)
	local bool = input.getBool(index)
	if (bool) then
		return 1
	else
		return 0
	end
end

function httpReply(port, request_body, response_body)
	response = response_body
	responseError = false
	
	if response_body == "storm.net.connected" then
		connected = true
		sending = false
		return
	end
	
	if string.starts(response_body, "storm.net.error") then
		responseError = true	
		sending = false
		return
	end

	if string.starts(response_body, "storm.net.ok") then
		sending = false
		return
	end
	
	if string.starts(response_body, "UVJSA") then
		sending = false
		qrCodePlayer = dec(response_body)
		return
	end
	
	if string.starts(response_body, "storm.net.data:") then
		sending = false
		local dataBase64 = string.sub(response_body, 16, -1)
		--print(dataBase64)
		dataBytes = dec(dataBase64)
		return
	end
	
	connected = false
	sending = false
end


-- Draw function that will be executed when this script renders to a screen
function onDraw()
	screen.setColor(255,255,255,230)
	screen.drawRectF(2,2,83,53)
	screen.setColor(0,0,0,255)

	if connected then
		screen.drawTextBox(5,5,80,50, "Scan QR code with phone to start controller.")
		
		if #qrCodePlayer > 0 then
			renderQrCode(qrCodePlayer, -1, -1, 1)
		end
	else
		screen.drawTextBox(5,5,80,50, "StormNet not detected, scan QRcode for installation, and run Application.")
		renderQrCode(qrCodeBytes, -1, -1, 1)
	end
end


function string.starts(String,Start)
   return string.sub(String,1,string.len(Start))==Start
end	


-- Draw qr bytes starting at h,v, with pixel width p
function renderQrCode(bytes, h, v, p) 

	local qrSize = string.unpack(">b", bytes, 5)
	local x = 0
	local y = 0
	
	if (h < 0) then
		h = screen.getWidth() - qrSize + h + 1
	end
	if (v < 0) then
		v = screen.getHeight() - qrSize + v + 1
	end

	-- Loop through bytes
	for i = 6,#bytes,1 do 
		local byte = string.unpack(">b", bytes, i)

		-- Loop through bits
		for b = 8,1,-1 do
			if (bit(byte, b)) then
				screen.setColor(0,0,0,255)
			else
				screen.setColor(255,255,255,255)
			end		
			screen.drawRectF(h+x*p,v+y*p,p,p)	
			x = x + 1
			if (x >= qrSize) then
				x = 0
				y = y + 1
			end			
			if (y >= qrSize) then
				return
			end		
		end
	end
end

function bit(x,bit)
	return (x % 2^bit - x % 2^(bit-1) > 0)
end

local b='ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@~'

-- encoding
function enc(data)
    return ((data:gsub('.', function(x) 
        local r,b='',x:byte()
        for i=8,1,-1 do r=r..(b%2^i-b%2^(i-1)>0 and '1' or '0') end
        return r;
    end)..'0000'):gsub('%d%d%d?%d?%d?%d?', function(x)
        if (#x < 6) then return '' end
        local c=0
        for i=1,6 do c=c+(x:sub(i,i)=='1' and 2^(6-i) or 0) end
        return b:sub(c+1,c+1)
    end)..({ '', '==', '=' })[#data%3+1])
end

-- decoding
function dec(data)
    data = string.gsub(data, '[^'..b..'=]', '')
    return (data:gsub('.', function(x)
        if (x == '=') then return '' end
        local r,f='',(b:find(x)-1)
        for i=6,1,-1 do r=r..(f%2^i-f%2^(i-1)>0 and '1' or '0') end
        return r;
    end):gsub('%d%d%d?%d?%d?%d?%d?%d?', function(x)
        if (#x ~= 8) then return '' end
        local c=0
        for i=1,8 do c=c+(x:sub(i,i)=='1' and 2^(8-i) or 0) end
        return string.char(c)
    end))
end


qrCodeBase64 = "UVJSACUAAAAAAAAAAAAAAAAAAAAAAAAA~q87@AQT5xBALrgqugF11pXQC6mvLoBBGu0EA~qqr@AABTwAAPK99OgFCXvcQC7fCUwBY8BNEA7@voYAUu3JHAI7UejgDApz8gAT6njQA84ZC4AhqdroACsvx0AHqn7@AABwxHwD@AvrQBBHwRgAuhBPsAXUrK5ALrK6SgEFWBqgD@sL4QAAAAAAAAAAAAAAAAAAAAAAAAAA"

qrCodeBytes = dec(qrCodeBase64)


