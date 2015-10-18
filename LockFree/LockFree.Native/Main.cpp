#include"RMBST.h"
#include <iostream>
#include <ctime>
#include <iostream>
#include <process.h>
#include <windows.h> 
#include <stdio.h> 
#include <atomic>
#include <vector> 
#include"OutputUtils.h"
using namespace std;

int NUM_OF_THREADS;
int findPercent;
int insertPercent;
int deletePercent;
unsigned long keyRange;
double MOPS;
volatile bool start = false;
volatile bool stop = false;
volatile bool steadyState = false;

struct tArgs** tArgs;

static inline unsigned long getRandom()
{
	return (unsigned long)(rand() % keyRange + 2);
}

double RealElapsedTime(void) { // granularity about 50 microsecs on my machine
	static LARGE_INTEGER freq, start;
	LARGE_INTEGER count;
	if (!QueryPerformanceCounter(&count))
		//FatalError("QueryPerformanceCounter");
		if (!freq.QuadPart) { // one time initialization
		if (!QueryPerformanceFrequency(&freq))
			//FatalError("QueryPerformanceFrequency");
			start = count;
		}
	return (double)(count.QuadPart - start.QuadPart) / freq.QuadPart;
}

void operateOnTree(void* tArgs)
{
	int chooseOperation;
	unsigned long lseed;
	unsigned long key;
	struct tArgs* tData = (struct tArgs*) tArgs;
	srand((unsigned)time(NULL));

	tData->newNode = NULL;
	tData->isNewNodeAvailable = false;
	tData->readCount = 0;
	tData->successfulReads = 0;
	tData->unsuccessfulReads = 0;
	tData->readRetries = 0;
	tData->insertCount = 0;
	tData->successfulInserts = 0;
	tData->unsuccessfulInserts = 0;
	tData->insertRetries = 0;
	tData->deleteCount = 0;
	tData->successfulDeletes = 0;
	tData->unsuccessfulDeletes = 0;
	tData->deleteRetries = 0;
	tData->seekRetries = 0;
	tData->seekLength = 0;

	while (!start)
	{
	}

	while (!steadyState)
	{
		chooseOperation = rand() % 100;
		key = getRandom();

		if (chooseOperation < findPercent)
		{
			search(tData, key);
		}
		else if (chooseOperation < insertPercent)
		{
			insert(tData, key);
		}
		else
		{
			remove(tData, key);
		}
	}

	tData->readCount = 0;
	tData->successfulReads = 0;
	tData->unsuccessfulReads = 0;
	tData->readRetries = 0;
	tData->insertCount = 0;
	tData->successfulInserts = 0;
	tData->unsuccessfulInserts = 0;
	tData->insertRetries = 0;
	tData->deleteCount = 0;
	tData->successfulDeletes = 0;
	tData->unsuccessfulDeletes = 0;
	tData->deleteRetries = 0;
	tData->seekRetries = 0;
	tData->seekLength = 0;

	while (!stop)
	{
		chooseOperation = rand() % 100;
		key = getRandom();

		if (chooseOperation < findPercent)
		{
			tData->readCount++;
			search(tData, key);
		}
		else if (chooseOperation < insertPercent)
		{
			tData->insertCount++;
			insert(tData, key);
		}
		else
		{
			tData->deleteCount++;
			remove(tData, key);
		}
	}

	_endthread();
}

int main(int argc, char *argv[])
{
	unsigned long lseed;
	//get run configuration from command line
	NUM_OF_THREADS = atoi(argv[1]);
	findPercent = atoi(argv[2]);
	insertPercent = findPercent + atoi(argv[3]);
	deletePercent = insertPercent + atoi(argv[4]);

	int warmupTime = 20; //miliseconds warmup
	int runTime = atoi(argv[5])*1000; //miliseconds runtime

	keyRange =  (unsigned long)atol(argv[6]) - 1;
	lseed = (unsigned long)atol(argv[7]);
	tArgs = (struct tArgs**) malloc(NUM_OF_THREADS * sizeof(struct tArgs*));

	createHeadNodes(); //Initialize the tree. Must be called before doing any operations on the tree

	struct tArgs* initialInsertArgs = (struct tArgs*) malloc(sizeof(struct tArgs));
	initialInsertArgs->successfulInserts = 0;
	initialInsertArgs->newNode = NULL;
	initialInsertArgs->isNewNodeAvailable = false;

	executionRecord executionRecord;
	executionRecord.searchPercentage = findPercent;
	executionRecord.insertPercentage = atoi(argv[3]);
	executionRecord.removePercentage = atoi(argv[4]);
	executionRecord.runTime = runTime;
	executionRecord.threadCount = NUM_OF_THREADS;
	executionRecord.keySpace = keyRange;

	while (initialInsertArgs->successfulInserts < keyRange / 2) //populate the tree with 50% of keys
	{
		insert(initialInsertArgs, getRandom());
	}

	for (int i = 0; i < NUM_OF_THREADS; i++)
	{
		tArgs[i] = (struct tArgs*) malloc(sizeof(struct tArgs));
		tArgs[i]->tId = i;
	}

	for (int i = 0; i < NUM_OF_THREADS; i++)
	{
		_beginthread(operateOnTree, 0, (void*)tArgs[i]);
	}

	start = true; 										//start operations
	Sleep(warmupTime); //warmup
	steadyState = true;
	Sleep(runTime);
	stop = true;										//stop operations

	assert(isValidTree());

	writeToFile("output.csv", executionRecord, tArgs);
}
