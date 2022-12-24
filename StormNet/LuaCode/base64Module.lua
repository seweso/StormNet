﻿-- Base64Module.lua Small module to convert to/from Base64. 
-- https://lua.flaffipony.rocks/?id=Fn0yaZGJSZ
-- 
-- Adapted from this stackoverflow answer: https://stackoverflow.com/a/35303321
-- mainly fixed that it can be minified, and created a class/module which you can load without interference. 
--

function base64Module(chars)local n,g,e={'','==','='},string.gsub,string.sub;return{enc=function(f)return g(g(f,'.',function(b)local d,j='',string.byte(b)for a=8,1,-1 do d=d..(j%2^a-j%2^(a-1)>0 and'1'or'0')end;return d end)..'0000','%d%d%d?%d?%d?%d?',function(b)if#b<6 then return''end;local c=0;for a=1,6 do c=c+(e(b,a,a)=='1'and 2^(6-a)or 0)end;return e(chars,c+1,c+1)end)..n[#f%3+1]end,dec=function(f)f=g(f,'[^'..chars..'=]','')return g(g(f,'.',function(b)if b=='='then return''end;local d,e='',string.find(chars,b)-1;for a=6,1,-1 do d=d..(e%2^a-e%2^(a-1)>0 and'1'or'0')end;return d end),'%d%d%d?%d?%d?%d?%d?%d?',function(b)if#b~=8 then return''end;local c=0;for a=1,8 do c=c+(e(b,a,a)=='1'and 2^(8-a)or 0)end;return string.char(c)end)end}end;

-- Usage for normal Base64 encoding
base64=base64Module('ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/')

-- Url friendly base64 encoding
base64X=base64Module('ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@~')

-- Test script
qrCodeBase64 = "UVJSACUAAAAAAAAAAAAAAAAAAAAAAAAA~q87@AQT5xBALrgqugF11pXQC6mvLoBBGu0EA~qqr@AABTwAAPK99OgFCXvcQC7fCUwBY8BNEA7@voYAUu3JHAI7UejgDApz8gAT6njQA84ZC4AhqdroACsvx0AHqn7@AABwxHwD@AvrQBBHwRgAuhBPsAXUrK5ALrK6SgEFWBqgD@sL4QAAAAAAAAAAAAAAAAAAAAAAAAAA"
qrCodeBytes = base64X.dec(qrCodeBase64)
encodedAgain = base64X.enc(qrCodeBytes)

if (qrCodeBase64 == encodedAgain) then
	print("working!")
else
	print("somethings iffy!")
end
pause()