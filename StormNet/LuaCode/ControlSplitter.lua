-- ControlSplitter.lua Used to show QrCode to open mobile controller 
-- https://lua.flaffipony.rocks/?id=A_c75TCh6-

playerNr=math.floor(property.getNumber("PlayerNr"))
nrOfPlayers=math.floor(property.getNumber("NrOfPlayers"))

rateLimit = property.getNumber("RateLimit")
requestNr = rateLimit
connected = false
sending = false
response = "..."
responseError = false
qrCodePlayer = ""

function onTick()
	-- Rate limiter for requests
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
	
	-- Load QRcode
	if #qrCodePlayer == 0 then
		local url = "http://[ipAddress]:18146/controller?playerNr=" .. playerNr .. "&nrOfPlayers=" .. nrOfPlayers
		stormGet("/qrcode?href=" .. urlencode(url))
		return
	end	
end

-- Draw function that will be executed when this script renders to a screen
function onDraw()
	if connected then
		screen.setColor(255,255,255,230)
		screen.drawRectF(2,2,83,53)
		screen.setColor(0,0,0,255)
		screen.drawTextBox(5,5,80,50, "Player " .. playerNr .. ". Scan QR code with phone to start controller.")
		
		if #qrCodePlayer > 0 then
			renderQrCode(qrCodePlayer, -1, -1, 1)
		end
	else
		screen.setColor(255,0,0,150)
		screen.drawRectF(0,0,999,99)
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
	
	connected = false
	sending = false
end


-- Encode an url
function urlencode(url)
  if url == nil then
    return
  end
  url = url:gsub("\n", "\r\n")
  url = url:gsub("([^%w ])", function (c) return string.format("%%%02X", string.byte(c))end)
  url = url:gsub(" ", "+")
  return url
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




