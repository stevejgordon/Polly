﻿using System;
using System.Threading;
using Polly.Utilities;

namespace Polly.Retry
{
    internal partial class RetryStateWaitAndRetryForeverWithCount<TResult> : IRetryPolicyState<TResult>
    {
        private int _errorCount;
        private readonly Func<int, DelegateResult<TResult>, Context, TimeSpan> _sleepDurationProvider;
        private readonly Action<DelegateResult<TResult>, int, TimeSpan, Context> _onRetry;
        private readonly Context _context;

        public RetryStateWaitAndRetryForeverWithCount(Func<int, DelegateResult<TResult>, Context, TimeSpan> sleepDurationProvider, Action<DelegateResult<TResult>, int, TimeSpan, Context> onRetry, Context context)
        {
            _sleepDurationProvider = sleepDurationProvider;
            _onRetry = onRetry;
            _context = context;
        }

        public bool CanRetry(DelegateResult<TResult> delegateResult, CancellationToken cancellationToken)
        {
            if (_errorCount < int.MaxValue)
            {
                _errorCount += 1;
            }

            TimeSpan waitTimeSpan = _sleepDurationProvider(_errorCount, delegateResult, _context);

            _onRetry(delegateResult, _errorCount, waitTimeSpan, _context);

            SystemClock.Sleep(waitTimeSpan, cancellationToken);
            
            return true;
        }        
    }
}
