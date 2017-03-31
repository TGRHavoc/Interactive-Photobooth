import time
import sys

def run(maxIterations = 5):
	i = 0
	while i < maxIterations:
		i = i+1
		print("Iteration %d of %d" % (i, maxIterations))
		time.sleep(1)

if __name__ == "__main__":
	if len(sys.argv) > 1:
		run(int(sys.argv[1]))
	else:
		run()
	

