using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

/// <summary>
/// 计时器状态
/// </summary>
public enum TimerStation
{
   noWorking,
   isWorking,
   doneWorking
}

/// <summary>
/// 单个计时器，用于延迟触发事件
/// </summary>
public class GameTimer
{
   private double startTime;
   private bool _isStopping;
   public TimerStation _timerStation;
   private Action _task;
   public bool _isRealing;

   public GameTimer()
   {
      InitTimer();
   }

   /// <summary>
   /// 开启计时器，并初始化属性。
   /// </summary>
   /// 是否收到scale的影响。
   /// <param name="isRealing"></param>
   /// 延迟时间
   /// <param name="time"></param>
   /// 回调函数
   /// <param name="task"></param>
   public void StartTimer(bool isRealing, float time, Action task)
   {
      startTime = time;
      _isRealing = isRealing;
      _task = task;
      _timerStation = TimerStation.isWorking;
      _isStopping = false;
   }

   /// <summary>
   /// 更新计时器状态
   /// </summary>
   public void UpdateTimer()
   {
      if (_timerStation != TimerStation.isWorking || _isStopping) return;

      startTime -= _isRealing ? Time.unscaledTimeAsDouble : Time.deltaTime;
      if (startTime <= 0)
      {
         _task?.Invoke();
         _isStopping = true;
         _timerStation = TimerStation.doneWorking;
      }
   }

   /// <summary>
   /// 初始化计时器
   /// </summary>
   public void InitTimer()
   {
      _timerStation = TimerStation.noWorking;
      startTime = 0f;
      _isStopping = true;
      _task = null;
      _isRealing = false;
   }
}
