import matplotlib.pyplot as pit

def LineToArray(line):
	line = line.replace(',', '.')
	LineMass = line.rsplit(' ')
	LineMass.remove('')
	LineMass.remove('')
	#LineMass.remove('&&&')
	return LineMass

def ReturnLineArray(Text):
	LineParsed = Text.rsplit('\n')
	return LineParsed

with open('test.txt', 'r') as f:
	text = f.read()
points = 10000                 #Количество точек, выведенное. Считаются то все)
#print(ReadLineFromFile("test.txt"))
fig = pit.figure(figsize=(10,7))
az = pit.axes(projection = '3d')
a = ReturnLineArray(text)
#print(a[0])

MyArrayData = []
i = 0
while(i<len(a)):
	add = LineToArray(a[i])
	LineMass = [[float(i) for i in add[2:(add.index("&&&"))]] , [float(i) for i in add[(add.index("&&&"))+1:len(a)]]]
	MyArrayData.append(LineMass)
	i+=1
a = []	
#print(MyArrayData[0])
x = []
y = []
z = []
i=0
#print (len(MyArrayData[100][0]))
#print(len(MyArrayData[100][1]))
j=0
while(i<len(MyArrayData)):
	while(j < len(MyArrayData[i][1])):
		if(len(MyArrayData[i][0]) == len(MyArrayData[i][1])):
			x.append(float(MyArrayData[i][0][j]))
			y.append(float(MyArrayData[i][1][j]))
			z.append(i*0.1)
		j+=1
	j = 0
	i+=1
#print(x)
#print(MyArrayData[0][1][5])
#az.plot3D(x[0:points], y[0:points], z[0:points])
az.scatter3D(x[0:points], y[0:points], z[0:points])
pit.show()
