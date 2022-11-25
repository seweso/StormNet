-- Used to read/write Composite data to/from Stormnet and mobile controllers

rateLimit = property.getNumber("RateLimit")
requestNr = rateLimit
connected = false
sending = false
response = "..."
responseError = false
qrCodePlayer = ""
dataBytes = ""

function onTick()
	-- Rate limiter
	if (requestNr < rateLimit) then
		requestNr = requestNr + 1
		return
	end
	requestNr = 0

	-- Check if sending (to prevent building a queue of requests)
	if (sending) then
		return
	end	
	sending = true	
	
	-- Connection test
	if not connected then
		stormGet("/?startup=true")
		return
	end
	
	if qrCodePlayer == "" then
		stormGet("/qrcode?href=http://[ipAddress]:18146/controller?player=1")
		return
	end	

	-- Read bytes from stormnet into outputs
	if string.len(dataBytes) > 0 then
		for i=1,32,1 do 
			output.setNumber(i, string.unpack("<d", dataBytes, i * 8 - 7))
			output.setBool(i, string.unpack("<b", dataBytes, 32 * 8 + i)==1)
		end
	end

	-- Write numbers / bools to bytes > url > stormnet
	local g = input.getNumber
	local b = getBoolAsByte
	local inputBytes = string.pack("<ddddddddddddddddddddddddddddddddbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb", 
	  	g(1),g(2),g(3),g(4),g(5),g(6),g(7),g(8),g(9),g(10),g(11),g(12),g(13),g(14),g(15),g(16),
	  	g(17),g(18),g(19),g(20),g(21),g(22),g(23),g(24),g(25),g(26),g(27),g(28),g(29),g(30),g(31),g(32),
  		b(1),b(2),b(3),b(4),b(5),b(6),b(7),b(8),b(9),b(10),b(11),b(12),b(13),b(14),b(15),b(16),
	  	b(17),b(18),b(19),b(20),b(21),b(22),b(23),b(24),b(2),b(26),b(27),b(28),b(29),b(30),b(31),b(32)	  
	  	)

	local inputBase64 = base64X.enc(inputBytes)
	stormGet("/controller?data=" .. inputBase64)
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



-- Respond to stormnet requests
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
		qrCodePlayer = base64X.dec(response_body)
		return
	end
	
	if string.starts(response_body, "storm.net.data:") then
		sending = false
		local dataBase64 = string.sub(response_body, 16, -1)
		dataBytes = base64X.dec(dataBase64)
		return
	end
	
	connected = false
	sending = false
end


-- Communicate with Stormnet via http
function stormGet(url)
	async.httpGet(18146, url)
end


-- Retrieve a bool as a byte (not very efficient, but it works)
function getBoolAsByte(index)
	local bool = input.getBool(index)
	if (bool) then
		return 1
	else
		return 0
	end
end


-- Check if strings starts with a string
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


-- Check a bit in a byte
function bit(x,bit)
	return (x % 2^bit - x % 2^(bit-1) > 0)
end

-- Base64 Module
function base64Module(chars)local n,g,e={'','==','='},string.gsub,string.sub;return{enc=function(f)return g(g(f,'.',function(b)local d,j='',string.byte(b)for a=8,1,-1 do d=d..(j%2^a-j%2^(a-1)>0 and'1'or'0')end;return d end)..'0000','%d%d%d?%d?%d?%d?',function(b)if#b<6 then return''end;local c=0;for a=1,6 do c=c+(e(b,a,a)=='1'and 2^(6-a)or 0)end;return e(chars,c+1,c+1)end)..n[#f%3+1]end,dec=function(f)f=g(f,'[^'..chars..'=]','')return g(g(f,'.',function(b)if b=='='then return''end;local d,e='',string.find(chars,b)-1;for a=6,1,-1 do d=d..(e%2^a-e%2^(a-1)>0 and'1'or'0')end;return d end),'%d%d%d?%d?%d?%d?%d?%d?',function(b)if#b~=8 then return''end;local c=0;for a=1,8 do c=c+(e(b,a,a)=='1'and 2^(8-a)or 0)end;return string.char(c)end)end}end;

-- Initialize Module
base64X=base64Module('ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@~')

-- Qrcode for installing StormNet --> Github
qrCodeBase64 = "UVJSACUAAAAAAAAAAAAAAAAAAAAAAAAA~q87@AQT5xBALrgqugF11pXQC6mvLoBBGu0EA~qqr@AABTwAAPK99OgFCXvcQC7fCUwBY8BNEA7@voYAUu3JHAI7UejgDApz8gAT6njQA84ZC4AhqdroACsvx0AHqn7@AABwxHwD@AvrQBBHwRgAuhBPsAXUrK5ALrK6SgEFWBqgD@sL4QAAAAAAAAAAAAAAAAAAAAAAAAAA"
qrCodeBytes = base64X.dec(qrCodeBase64)


