using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace FactoryZero.Threading
{
    public class MainThreadActionExecutor : MonoBehaviour
    {
        public static Queue<Action> mainThreadActions = new Queue<Action>();
        public static Queue<Action> alternateThreadActions = new Queue<Action>();

        public int threadCount;
        bool run;

        public TimeSpan threadSleepTime = TimeSpan.FromMilliseconds(100);

        // Thread[] threads;
        private void Start()
        {
            run = true;

            /*
            threads = new Thread[threadCount];

            for(int i = 0; i < threadCount; i++)
            {
                Thread t = new Thread(AltThread);
                t.Start();

                threads[i] = t;
            }
            */
        }

        ~MainThreadActionExecutor()
        {
            run = false;
        }

        private void OnDestroy()
        {
            run = false;
        }
        /*
        void AltThread()
        {
            while(run)
            {
                Thread.Sleep(threadSleepTime);

                DateTime startTime = DateTime.UtcNow;

                try
                {
                    if(alternateThreadActions.Count > 0)
                    {
                        Action action = alternateThreadActions.Dequeue();
                        action?.Invoke();
                    }

                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }

                DateTime stopTime = DateTime.UtcNow;
                Thread.Sleep(stopTime - stopTime);
            }
        }
        */

        void FixedUpdate()
        {
            while (mainThreadActions.Count > 0)
            {
                Action action = mainThreadActions.Dequeue();
                action?.Invoke();
            }
        }
    }
}
