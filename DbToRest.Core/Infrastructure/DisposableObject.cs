﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace DbToRest.Core.Infrastructure
{
    public abstract class DisposableObject : IDisposable
    {
        private bool _disposed = false;

        public virtual bool IsDisposed
        {
            [DebuggerStepThrough]
            get
            { return _disposed; }
        }

        [DebuggerStepThrough]
        protected void CheckDisposed()
        {
            if (IsDisposed)
            {
                throw Error.ObjectDisposed(GetType().FullName);
            }
        }

        [DebuggerStepThrough]
        protected void CheckDisposed(string errorMessage)
        {
            if (IsDisposed)
            {
                throw Error.ObjectDisposed(GetType().FullName, errorMessage);
            }
        }

        [DebuggerStepThrough]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                OnDispose(disposing);
            }
            _disposed = true;
        }

        protected abstract void OnDispose(bool disposing);

        protected static void DisposeEnumerable(IEnumerable enumerable)
        {
            if (!enumerable.IsNullOrEmpty())
            {
                foreach (object obj2 in enumerable)
                {
                    DisposeMember(obj2);
                }
                DisposeMember(enumerable);
            }
        }

        protected static void DisposeDictionary<K, V>(IDictionary<K, V> dictionary)
        {
            if (dictionary != null)
            {
                foreach (KeyValuePair<K, V> pair in dictionary)
                {
                    DisposeMember(pair.Value);
                }
                DisposeMember(dictionary);
            }
        }

        protected static void DisposeDictionary(IDictionary dictionary)
        {
            if (dictionary != null)
            {
                foreach (IDictionaryEnumerator pair in dictionary)
                {
                    DisposeMember(pair.Value);
                }
                DisposeMember(dictionary);
            }
        }

        protected static void DisposeMember(object member)
        {
            IDisposable disposable = member as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        ~DisposableObject()
        {
            Dispose(false);
        }
    }
}