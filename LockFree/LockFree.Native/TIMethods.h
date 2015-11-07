__inline void enqueue(treeData data, struct qroot* root);
__inline void enqueue_heuristic_sorted(treeData data, qroot* root);
__inline treeData* dequeue(qroot* root);
__inline treeData* peek(qroot* root);

__inline void seek(tArgs*, unsigned long, seekRecord*); 
__inline void findSuccessor(tArgs* t, seekRecord* s);

// ===================== PSEUDO-PRIORITY QUEUE UTILS ===================== 

__inline void enqueue(treeData data, qroot* root)
{
	qnode* oldTail;
	qnode* node = (qnode*) malloc(sizeof(qnode));
	node->data = data;
	
	do 
	{
		oldTail = getUnmarkedAddress(root->tail); //we check against the pure (unmarked) address
		oldTail->next = node;
	} while (root->tail.compare_exchange_strong(oldTail, node, std::memory_order_seq_cst));
}

__inline void enqueue_heuristic_sorted(treeData data, qroot* root)
{
	qnode* oldTail;
	qnode* oldHead;
	qnode* node = (qnode*) malloc(sizeof(qnode));
	node->data = data;

	do
	{
		oldHead = getUnmarkedAddress(root->head);
		oldTail = getUnmarkedAddress(root->tail);
		
		if (oldHead->data.depth > node->data.depth)
		{
			node->next = oldHead;

			if (root->head.compare_exchange_strong(oldHead, node, std::memory_order_seq_cst))
				return;
		}
		else
		{
			oldTail->next = node;

			if (root->tail.compare_exchange_strong(oldTail, node, std::memory_order_seq_cst))
				return;
		}

	} while (true);
}

__inline treeData* dequeue(qroot* root)
{
	qnode* oldHead;

	do
	{
		oldHead = root->head;

		if (isTagged(oldHead->next)) //we check that the address was not tagged
			return NULL;

	} while (root->head.compare_exchange_strong(oldHead, oldHead->next, std::memory_order_seq_cst));

	return &(oldHead->data);
}

__inline treeData* peek(qroot* root)
{
	qnode* oldHead;

	do
	{
		oldHead = root->head;

		if (isTagged(oldHead) && isTagged(oldHead->next)) //we check that the address was not tagged
			return NULL; 

	} while (isTagged(oldHead));

	return &(oldHead->data);
}


// ===================== BST METHODS ===================== 

__inline void seek(tArgs* t, node* root, unsigned long key, seekRecord* s)
{ 
	node* current = root;
	node* anchor;
	node* temp;
	bool isBoundary;
	s->key = key;

	while (true)
	{
		t->seekLength++;

		currentKey = getKey(current->markAndKey);
		
		if (key < currentKey)
		{
			temp = current->left;
		}
		else
		{
			temp = current->right;
			anchor = current;
		}

		isBoundary = isNull(temp);

		if (key == currentKey)
		{
			s->target = current;
			s->successor = ? ? ;

			return;
		}
		else if (isBoundary)
		{
			break;
		}
	}

	if ()

		t->seekRetries++;
}

__inline void findSuccessor(tArgs* t, seekRecord* s)
{
	node* parent = s->target; 
	node* successor = parent->left;
	bool isSuccessor = isNull(successor->left);

	while (!isSuccessor)
	{
		parent = successor;
		successor = successor->left;
		isSuccessor = isNull(successor->left);
	}

	s->successorParent = parent;
	s->successor = successor;
}


// ===================== BIT UTILS =====================  

__inline qnode* getUnmarkedAddress(qnode* p)
{
	return (qnode*)((uintptr_t)p &  ~ADDRESS_MASK);
}

__inline bool isTagged(qnode* p)
{
	return (uintptr_t)p == (uintptr_t)p &  ~ADDRESS_MASK;
}