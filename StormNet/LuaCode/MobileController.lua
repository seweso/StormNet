-- MobileController.lua Mobile controlle for phone
-- 

i2sliderv={x=39,y=33,w=25,h=91,v=0.5}
i9sliderh={x=120,y=108,w=137,h=33,v=0.5}

playerNumber = -1
i8 = -1

function onTick()
isP1 = input.getBool(1)
isP2 = input.getBool(2)

output.setBool(1, true)	
output.setBool(2, isP1)	
output.setBool(3, isP2)	


in1X = input.getNumber(3)
in1Y = input.getNumber(4)
in2X = input.getNumber(5)
in2Y = input.getNumber(6)

playerNumber = input.getNumber(7)
i8 = input.getNumber(8)

if isP1 and isInRectO(i2sliderv,in1X,in1Y) then
i2sliderv.v=((i2sliderv.y+i2sliderv.h)-in1Y)/i2sliderv.h
elseif isP2 and isInRectO(i2sliderv,in2X,in2Y) then
i2sliderv.v=((i2sliderv.y+i2sliderv.h)-in2Y)/i2sliderv.h
else
i2sliderv.v=0.5
end
if i2sliderv.v<0.1 then
i2sliderv.v=0
elseif i2sliderv.v>0.9 then
i2sliderv.v=1
end
output.setNumber(2,i2sliderv.v*2-1)

if isP1 and isInRectO(i9sliderh,in1X,in1Y) then
i9sliderh.v=(in1X-i9sliderh.x)/i9sliderh.w
elseif isP2 and isInRectO(i9sliderh,in2X,in2Y) then
i9sliderh.v=(in2X-i9sliderh.x)/i9sliderh.w
else
i9sliderh.v=0.5
end
if i9sliderh.v<0.1 then
i9sliderh.v=0
elseif i9sliderh.v>0.9 then
i9sliderh.v=1
end
output.setNumber(1,i9sliderh.v*2-1)

end

function onDraw()

setC(255,0,0)
screen.drawTextBox(0, 0, 100, 13, "i8 = "  .. i8, 0, 0)
setC(255,0,0)
screen.drawTextBox(100, 0, 100, 13, "Player "  .. string.format("%f",playerNumber), 0, 0)

setC(38,38,38)
screen.drawRectF(39,33,25,91)
setC(48,96,39)
screen.drawRectF(i2sliderv.x,i2sliderv.y,i2sliderv.w,i2sliderv.h)
setC(96,34,64)
screen.drawRectF(i2sliderv.x,(1-i2sliderv.v)*i2sliderv.h+i2sliderv.y,i2sliderv.w,(i2sliderv.v)*i2sliderv.h)
setC(38,38,38)
screen.drawRect(i2sliderv.x,i2sliderv.y,i2sliderv.w,i2sliderv.h)

setC(81,3,66)
screen.drawRectF(39,20,25,10)
setC(81,3,66)
screen.drawRectF(40,21,23,8)
setC(255,255,255)
screen.drawTextBox(39, 20, 25, 10, "Gas", 0, 0)

setC(53,30,93)
screen.drawRectF(120,94,137,13)
setC(53,30,93)
screen.drawRectF(121,95,135,11)
setC(255,255,255)
screen.drawTextBox(120, 94, 137, 13, "Steer", 0, 0)

setC(38,38,38)
screen.drawRectF(120,108,137,33)
setC(16,0,96)
screen.drawRectF(i9sliderh.x,i9sliderh.y,i9sliderh.w,i9sliderh.h)
setC(42,58,96)
screen.drawRectF(i9sliderh.x,i9sliderh.y,(i9sliderh.v)*i9sliderh.w,i9sliderh.h)
setC(38,38,38)
screen.drawRect(i9sliderh.x,i9sliderh.y,i9sliderh.w,i9sliderh.h)

end

function setC(r,g,b,a)
if a==nil then a=255 end
screen.setColor(r,g,b,a)
end

function isInRectO(o,px,py)
return px>=o.x and px<=o.x+o.w and py>=o.y and py<=o.y+o.h
end

