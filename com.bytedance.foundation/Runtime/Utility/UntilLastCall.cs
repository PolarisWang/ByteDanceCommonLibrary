/**************************************************************
 * FileName:	UntilLastCall.cs
 * Author:		王浩川
 * CreateTime:  2018-06-29 20:35
 * Copyright:   ByteDance
 * Description: 
 ***************************************************************/

using System;

namespace ByteDance.Foundation
{

    public class UntilLastCall
    {
        private static MyLogger logger = new MyLogger("UntilLastCall");

        public readonly Action Action;
        public int TotalCount { get; private set; }
        public int RefCount { get; private set; }
        public int InvocationCount { get; private set; }
        public bool Invoked { get; private set; }

        public UntilLastCall(Action action, int totalCount)
        {
            Assert.AssertAtLeast(totalCount, 1);

            this.Action = action;
            this.TotalCount = totalCount;
            Invoked = false;
        }

        private void increaseRefCount()
        {
            if (Invoked)
            {
                Assert.Fail("Unexpected referencing");
                return;
            }

            RefCount++;
            Assert.AssertAtMost(RefCount, TotalCount);
        }

        private void invoke()
        {
            if (Invoked)
            {
                Assert.Fail("Unexpected invocation.");
                return;
            }

            InvocationCount++;
            Assert.AssertAtMost(InvocationCount, RefCount);
            if (InvocationCount == TotalCount)
            {
                Action.InvokeSafely();
                Invoked = true;
            }
            else if (InvocationCount == RefCount)
            {
                //logger.Warn("invocation count reaches reference count before reaches total count. " +
                // 	"This implies some of the invocation is actially not asynchronous. " +
                // 	"Using this class in synchronous call is not recommended.");
            }
        }

        public void Invoke()
        {
            increaseRefCount();
            invoke();
        }

        public static implicit operator Action(UntilLastCall obj)
        {
            obj.increaseRefCount();
            return obj.invoke;
        }

    }

}
