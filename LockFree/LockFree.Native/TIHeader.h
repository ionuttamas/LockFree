#include<stdio.h>
#include <windows.h>
#include<stdlib.h>
#include<stdbool.h>
#include<math.h>
#include<time.h>
#include<stdint.h> 
#include<assert.h> 
#include <atomic> 

#define K 2
#define LEFT 0
#define RIGHT 1

#define MAX_KEY 0x7FFFFFFF
#define INF_R 0x0
#define INF_S 0x1
#define INF_T 0x7FFFFFFE
#define KEY_MASK 0x80000000
#define ADDRESS_MASK 15	

#define NULL_BIT 8
#define INJECT_BIT 4
#define DELETE_BIT 2
#define PROMOTE_BIT 1

typedef enum { INJECTION, DISCOVERY, CLEANUP } Mode;
typedef enum { SIMPLE, COMPLEX } Type;
typedef enum { DELETE_FLAG, PROMOTE_FLAG } Flag;

typedef struct treeData 
{
	int depth;
	unsigned long key;
} treeData;

typedef struct qnode
{
	std::atomic<qnode*> next;
	treeData data;
}qnode;

typedef struct qroot
{
	std::atomic<qnode*> head; //Dequeue and peek will be performed from head
	std::atomic<qnode*> tail; //Enqueue will be performed to tail
}qroot;

typedef struct node
{ 
	std::atomic<unsigned long> markAndKey;
	std::atomic<node*> left;
	std::atomic<node*> right;
}node;

typedef struct seekRecord
{
	node* target;
	node* successor;
	node* successorParent;
	unsigned long key;
}seekRecord;

typedef struct tArgs
{
	int tId;
	unsigned long lseed;
	unsigned long readCount;
	unsigned long successfulReads;
	unsigned long unsuccessfulReads;
	unsigned long readRetries;
	unsigned long insertCount;
	unsigned long successfulInserts;
	unsigned long unsuccessfulInserts;
	unsigned long insertRetries;
	unsigned long deleteCount;
	unsigned long successfulDeletes;
	unsigned long unsuccessfulDeletes;
	unsigned long deleteRetries;
	struct node* newNode;
	bool isNewNodeAvailable;
	struct seekRecord targetRecord;
	struct seekRecord pSeekRecord; 
	unsigned long seekRetries;
	unsigned long seekLength;
}tArgs;

typedef struct executionRecord
{
	int keySpace;
	int searchPercentage; //0-100 range
	int insertPercentage; //0-100 range
	int removePercentage; //0-100 range
	int runTime;		  //in milliseconds
	int threadCount;
};

__inline bool search(struct tArgs*, unsigned long);
__inline bool insert(struct tArgs*, unsigned long);
__inline bool remove(struct tArgs*, unsigned long);

void createHeadNodes();
unsigned long size();
void printKeys();
bool isValidTree();