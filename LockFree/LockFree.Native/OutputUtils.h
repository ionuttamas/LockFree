#include <iostream>
#include <fstream>
#include <stdio.h>
using namespace std;

void writeToFile(char* filename, executionRecord execution, tArgs** results)
{
	ofstream file;

	unsigned long totalReadCount = 0;
	unsigned long totalSuccessfulReads = 0;
	unsigned long totalUnsuccessfulReads = 0;
	unsigned long totalReadRetries = 0;
	unsigned long totalInsertCount = 0;
	unsigned long totalSuccessfulInserts = 0;
	unsigned long totalUnsuccessfulInserts = 0;
	unsigned long totalInsertRetries = 0;
	unsigned long totalDeleteCount = 0;
	unsigned long totalSuccessfulDeletes = 0;
	unsigned long totalUnsuccessfulDeletes = 0;
	unsigned long totalDeleteRetries = 0;
	unsigned long totalSeekRetries = 0;
	unsigned long totalSeekLength = 0;

	for (int i = 0; i < execution.threadCount; i++)
	{
		totalReadCount += results[i]->readCount;
		totalSuccessfulReads += results[i]->successfulReads;
		totalUnsuccessfulReads += results[i]->unsuccessfulReads;
		totalReadRetries += results[i]->readRetries;

		totalInsertCount += results[i]->insertCount;
		totalSuccessfulInserts += results[i]->successfulInserts;
		totalUnsuccessfulInserts += results[i]->unsuccessfulInserts;
		totalInsertRetries += results[i]->insertRetries;
		totalDeleteCount += results[i]->deleteCount;
		totalSuccessfulDeletes += results[i]->successfulDeletes;
		totalUnsuccessfulDeletes += results[i]->unsuccessfulDeletes;
		totalDeleteRetries += results[i]->deleteRetries;
		totalSeekRetries += results[i]->seekRetries;
		totalSeekLength += results[i]->seekLength;
	}
	unsigned long totalOperations = totalReadCount + totalInsertCount + totalDeleteCount;
	double mops = totalOperations / (execution.runTime*1000.0);
	char buffer[200];
	sprintf_s(buffer, "%f,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d", mops, totalOperations, execution.runTime * 1000, execution.threadCount, execution.searchPercentage, execution.insertPercentage, execution.removePercentage, execution.keySpace, totalSeekRetries, totalInsertRetries, totalDeleteRetries);
	
	file.open(filename, std::ios_base::app);
	file << buffer << endl;
	file.close();
}