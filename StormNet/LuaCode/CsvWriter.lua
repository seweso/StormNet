-- CsvWriter.lua Used to write CsvValues to disk

first = true
send = false
tick = 0
newFile = true
connected = false
tryConnect = true
response = "..."
recordsSaved = 0
responseError = false


-- Tick function that will be executed every logic tick
function onTick()
	bool1 = input.getBool(1)
	output.setBool(1, connected)
	output.setBool(2, responseError)
	output.setNumber(1, recordsSaved)	
		
	-- Run initialisation
    if first then
		first = false
		filename = property.getText("Filename")
		interval = property.getNumber("Interval In Seconds") * 60
		writeOnOn = property.getBool("Write when")
		numberOfFields = property.getNumber("Number Of Fields")
		lastbool = bool1
	end
	
	if tryConnect then
		tryConnect = false	
		-- test the connection
		async.httpGet(18146, "/?startup=true")
	end

	if writeOnOn then
		-- Write when bool flips from ON>OFF or OFF>ON
		if lastbool ~= bool1 then
			send = true
			lastbool = bool1
		end
	else
		-- Write when interval elapsed
		tick = tick + 1
		if tick > interval then
			send = true
			tick = 0			
		end
	end
	
	
	-- Send data
	if send then
	    send = false
	
		fields = ""
		for i=1,numberOfFields do 
			fieldName = property.getText("FieldName" .. i)
			if fieldName == "" then
				fieldName = "F" .. i
			end
			fields = fields .. "&" .. fieldName .. "=" .. input.getNumber(i)
		end
	
		async.httpGet(18146, "/?filename=" .. filename .. "&newFile=" .. tostring(newFile) .. fields)
		newFile = false
	end
end


function httpReply(port, request_body, response_body)
	response = response_body
	responseError = false
	
	if response_body == "storm.net.connected" then
		connected = true
		return
	end

	if response_body == "storm.net.saved" then
		recordsSaved = recordsSaved + 1
		return
	end
	
	if string.starts(response_body, "storm.net.error") then
		responseError = true	
		return
	end
	
	connected = false
	tryConnect = true
end

-- Draw function that will be executed when this script renders to a screen
function onDraw()
	if connected then
		screen.drawText(0,0,"Status: Connected")
	else
		screen.drawText(0,0,"Status: Connecting")	
	end
	screen.drawText(0,10,"Response:" .. response)
	screen.drawText(0,20,"Saved:" .. recordsSaved)
end
	

function string.starts(String,Start)
   return string.sub(String,1,string.len(Start))==Start
end	