using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PanelSemi_Coloradjustment
{
    /// <summary>實作 <see cref="ICommand"/> 的可帶入參數之執行命令</summary>
    /// <remarks>參考自: http://aminggo.idv.tw/blog/?p=1522 </remarks>
    [Serializable]
    public class DelegateCommand<T> : ICommand
    {

        #region Fields
        /// <summary>查詢是否可執行命令之方法</summary>
        private readonly Func<T, bool> rCanExec;
        /// <summary>欲執行的命令內容</summary>
        private readonly Action<T> rExec;
        /// <summary>指出是否正在執行中</summary>
        private bool mIsExec;
        #endregion

        #region Constructors
        /// <summary>初始化命令</summary>
        /// <param name="exec">欲執行的命令內容</param>
        public DelegateCommand(Action<T> exec) : this(null, exec)
        {

        }

        /// <summary>初始化命令</summary>
        /// <param name="canExec">查詢是否可執行命令之方法</param>
        /// <param name="exec">欲執行的命令內容</param>
        public DelegateCommand(Func<T, bool> canExec, Action<T> exec)
        {
            if (exec is null)
            {
                throw new ArgumentNullException("exec");
            }
            rCanExec = canExec;
            rExec = exec;
            mIsExec = false;
        }
        #endregion

        #region ICommand Implements
        /// <summary>可執行命令的條件變更事件</summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (rCanExec != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (rCanExec != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
        }

        /// <summary>查詢是否可執行命令</summary>
        /// <param name="parameter">附加參數</param>
        /// <returns>(true)可執行 (false)不可執行</returns>
        bool ICommand.CanExecute(object parameter)
        {
            return !mIsExec && (rCanExec?.Invoke((T)parameter) ?? true);  //沒有委派方法就直接回傳可執行
        }

        /// <summary>執行命令</summary>
        /// <param name="parameter">附加參數</param>
        void ICommand.Execute(object parameter)
        {
            /* 將旗標 On 起來 */
            mIsExec = true;
            try
            {
                /* 重新呼叫檢查 CanExecute */
                CommandManager.InvalidateRequerySuggested();
                /* 執行內容 */
                rExec?.Invoke((T)parameter);
            }
            finally
            {
                /* 關閉旗標 */
                mIsExec = false;
                /* 重新呼叫檢查 CanExecute */
                CommandManager.InvalidateRequerySuggested();
            }
        }

        /// <summary>查詢是否可執行命令</summary>
        /// <param name="parameter">附加參數</param>
        /// <returns>(true)可執行 (false)不可執行</returns>
        public bool CanExecute(T parameter)
        {
            return rCanExec?.Invoke(parameter) ?? true;
        }

        /// <summary>執行命令</summary>
        /// <param name="parameter">附加參數</param>
        public void Execute(T parameter)
        {
            rExec?.Invoke(parameter);
        }
        #endregion

        #region Public Operations
        /// <summary>觸發可執行狀態變更，會讓父層重新檢查 <see cref="ICommand.CanExecute(object)"/></summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
        #endregion
    }
}
