"""Compare actual battle sprites at one world-to-screen scale; source art is read-only."""
from pathlib import Path
import ast,json,re,struct,zlib
from BuffmageAtlas import read_png
ROOT=Path(__file__).resolve().parents[1]
OUT=ROOT/"Assets/Resources/Heroes/Buffmage/Animations"
m=json.loads((OUT/"BuffmageAtlas.json").read_text())
tree=ast.parse((ROOT/"Tools/Preview-BuffmageAnimations.py").read_text())
font=next(ast.literal_eval(node.value) for node in tree.body if isinstance(node,ast.Assign) and any(isinstance(t,ast.Name) and t.id=="font" for t in node.targets))
font.update({
"F":"11111 10000 10000 11110 10000 10000 10000",
"G":"01111 10000 10000 10111 10001 10001 01110",
"S":"01111 10000 10000 01110 00001 00001 11110",
"V":"10001 10001 10001 10001 10001 01010 00100",
})
w,h=660,250
canvas=bytearray(bytes([31,29,28,255])*(w*h))
def rectangle(x,y,ww,hh,color):
 for yy in range(max(0,y),min(h,y+hh)):
  left,right=max(0,x),min(w,x+ww)
  canvas[(yy*w+left)*4:(yy*w+right)*4]=bytes(color)*(right-left)
def text(label,cx,y):
 for n,char in enumerate(label):
  for dy,row in enumerate(font[char].split()):
   for dx,bit in enumerate(row):
    if bit=="1":rectangle(cx-len(label)*12//2+n*12+dx*2,y+dy*2,2,2,[225,204,161,255])
def sprite(path,rect,ppu,root_scale,panel):
 sw,sh,rows=read_png(path)
 x,y,rw,rh=rect
 top=sh-y-rh
 points=[(xx,yy) for yy in range(rh) for xx in range(rw) if rows[top+yy][(x+xx)*4+3]>=192]
 minx,maxx=min(a for a,b in points),max(a for a,b in points)
 miny,maxy=min(b for a,b in points),max(b for a,b in points)
 factor=26*root_scale/ppu
 dw,dh=round((maxx-minx+1)*factor),round((maxy-miny+1)*factor)
 ox=panel*220+(220-dw)//2
 oy=201-dh
 for yy in range(dh):
  sy=top+miny+min(maxy-miny,int(yy/factor))
  for xx in range(dw):
   sx=x+minx+min(maxx-minx,int(xx/factor))
   pixel=rows[sy][sx*4:sx*4+4]
   if pixel[3]>=192:
    at=((oy+yy)*w+ox+xx)*4
    canvas[at:at+4]=pixel[:3]+b'\xff'
 for xx in range(panel*220+25,(panel+1)*220-25):rectangle(xx,203,1,1,[111,87,60,255])
 return (maxy-miny+1)*root_scale/ppu
knight=ROOT/"Assets/Knight/Sprites/HeroKnight.png"
km=Path(str(knight)+".meta").read_text().split("name: HeroKnight_0")[1].split("alignment:")[0]
kr=[int(re.search(r'\b'+key+r': (\d+)',km)[1]) for key in ("x","y","width","height")]
for i,label in enumerate(("KNIGHT","PREVIOUS","BUFFMAGE")):
 rectangle(i*220+4,4,212,h-8,[43,36,33,255]);text(label,i*220+110,20)
heights=[
 sprite(knight,kr,32,3,0),
 sprite(OUT/"BuffmageSheet-v1.png",[0,905,181,181],56,1,1),
 sprite(OUT/m["sheet"],[m["frames"][0][k] for k in ("x","y","width","height")],m["pixelsPerUnit"],1,2)
]
def chunk(kind,data):
 return struct.pack(">I",len(data))+kind+data+struct.pack(">I",zlib.crc32(kind+data)&0xffffffff)
raw=b''.join(b'\0'+canvas[y*w*4:(y+1)*w*4] for y in range(h))
png=b'\x89PNG\r\n\x1a\n'+chunk(b'IHDR',struct.pack(">IIBBBBB",w,h,8,6,0,0,0))+chunk(b'IDAT',zlib.compress(raw,9))+chunk(b'IEND',b'')
dest=ROOT/"output/buffmage/scale-facing-comparison.png"
dest.parent.mkdir(parents=True,exist_ok=True);dest.write_bytes(png)
print("Visible heights in world units:",[round(v,3) for v in heights])
print(dest)
