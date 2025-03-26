using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimerManager : MonoSigleton<TimerManager>
{
   [SerializeField] private float TimerCount;
   private Queue<GameTimer> noWorkTimers = new Queue<GameTimer>();
   private List<GameTimer> workTimers = new List<GameTimer>();

   /// <summary>
   /// 从池中获得并开启一个计时器
   /// </summary>
   /// <param name="time"></param>
   /// <param name="callback"></param>
   /// <param name="isRealing"></param>
   /// <returns></returns>
   public GameTimer GetTimer(float time, Action callback, bool isRealing = false)
   {
       if (workTimers.Count >= TimerCount) return null;
       
       if (noWorkTimers.Count == 0) CreateTimer();

       GameTimer gameTimer = noWorkTimers.Dequeue();
       gameTimer.StartTimer(isRealing, time, callback);
       workTimers.Add(gameTimer);
       
       return gameTimer;
   }

   /// <summary>
   /// 创建一个计时器
   /// </summary>
   public void CreateTimer()
   {
       noWorkTimers.Enqueue(new GameTimer());
   }

   /// <summary>
   /// 取消计时器
   /// </summary>
   /// <param name="gameTimer"></param>
   public void UnRigisterTimer(GameTimer gameTimer)
   {
       if (gameTimer._timerStation != TimerStation.isWorking) return;
       
       PushPool(gameTimer);
   }

   /// <summary>
   /// 统一管理计时器的运行逻辑，并缓存工作后的计时器。
   /// </summary>
   public void Update()
   {
       if (workTimers.Count <= 0) return;
       
       foreach (var Timer in workTimers.ToList())
       {
           Timer.UpdateTimer();
           if (Timer._timerStation == TimerStation.doneWorking)
           {
               PushPool(Timer);
           }
       }
   }

   /// <summary>
   /// 入池
   /// </summary>
   /// <param name="gameTimer"></param>
   private void PushPool(GameTimer gameTimer)
   {
       gameTimer.InitTimer();
       workTimers.Remove(gameTimer);
       noWorkTimers.Enqueue(gameTimer);
   }
   
}
