-- Documentation.lua Lua code which explains how to build lua code
-- https://lua.flaffipony.rocks/?id=qjzIPO504r

function onTick()
	isPressed1 = input.getBool(1)
	isPressed2 = input.getBool(2)
	
	screenWidth = input.getNumber(1)
	screenHeight = input.getNumber(2)
	
	input1X = input.getNumber(3)
	input1Y = input.getNumber(4)
	input2X = input.getNumber(5)
	input2Y = input.getNumber(6)
end    
     
function onDraw()
	if isPressed1 then
	    screen.setColor(0, 255, 0)
	    screen.drawCircleF(input1X, input1Y, 4)
	end
	
	if isPressed2 then
	    screen.setColor(255, 0, 0)
	    screen.drawCircleF(input2X, input2Y, 4)
	end
	
	if isPressed1 and isPressed2 then
		screen.setColor(0, 0, 255)
	    screen.drawLine(input1X, input1Y, input2X, input2Y)
	end

	screen.setColor(0,255,0)
	screen.drawTextBox(0,0,screen.getWidth(),screen.getHeight(), "Yes this is an app written in Stormworks lua. Do with that as you like :)\n\nYou can load your own script by creating and sharing one on https://lua.flaffipony.rocks/ and changing ?id=xxxx here in the url.\n\nMulti-touch works different than stormworks, variables can be loaded via the url and this has websocket/signalR support (so it can communicate with Storm.Net). ")
end
