/***********************************************************
 * UpdateRegisterManager
 * 功能描述：
 *     注册简易Update事件
 * 适用范围：
 *     组建功能需要全局Update逻辑的适合使用使用这个功能
 * 不适用范围:
 *     与时序强关联的逻辑不建议使用
 * 注意事项：
 *     Update的时间点在Globals的Manager的Tick时间，请明确调用时间点
 ***********************************************************/

using System.Collections.Generic;
using ByteDance.Foundation;

namespace ByteDance.ComLayer
{
    using UpdateID = System.Int32;
    using LoopCount = System.UInt32;

    public sealed class UpdateRegisterManager : IByteDanceGlobal
    {
        #region Field
        public const int InvalidId = -1;

        private static MyLogger logger = new MyLogger("UpdateRegisterManager");

        private List<UpdateDataStruct> updateList = new List<UpdateDataStruct>(); // Update 方法注册移除频率较低，所以使用List存储
        private List<UpdateID> toRemoveIdList = new List<int>();
        private int counter = 0;
        #endregion

        #region Interface
        public void Awake()
        {
        }

        public void Update()
        {
            DoProcess();
            // Update
            for (int i = 0;i<updateList.Count;i++)
                updateList[i].DoAction();
            DoProcess();
        }

        public void Dispose()
        {
            updateList.Clear();
            toRemoveIdList.Clear();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// 注册Update事件
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public UpdateID RegistUpdate(System.Action<UpdateID, LoopCount> action)
        {
            var id = counter++;
            UpdateDataStruct data = new UpdateDataStruct()
            {
                UpdateId = id,
                Action = action
            };
            updateList.Add(data);
            return id;
        }

        /// <summary>
        /// 解注册Update事件
        /// </summary>
        /// <param name="updateId"></param>
        /// <returns></returns>
        public void UnRegistUpdate(UpdateID updateId)
        {
            toRemoveIdList.AddSafely(updateId);
        }
        #endregion

        #region Private methods

        private void DoProcess()
        {
            // 开始移除，防止List动态改变
            for (int i = 0; i < toRemoveIdList.Count; i++)
            {
                bool isRemoveSuccess = false;
                for (int index = 0; index < updateList.Count; index++)
                    if (updateList[index].UpdateId == toRemoveIdList[i])
                    {
                        isRemoveSuccess = true;
                        updateList.RemoveAt(index);
                        break;
                    }
                if (!isRemoveSuccess)
                    logger.LogError("Remove Register Update failed, UpdateId = {0}".F(toRemoveIdList[i]));
            }

            toRemoveIdList.Clear();
        } 
        #endregion

        struct UpdateDataStruct
        {
            public int UpdateId;
            public System.Action<UpdateID, LoopCount> Action;
            private LoopCount _actionCounter;

            public void DoAction()
            {
                Action.InvokeSafely(UpdateId, ++_actionCounter);
            }
        }
    }
}