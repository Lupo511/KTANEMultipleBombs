using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultipleBombsAssembly
{
    public static class DelegateUtils
    {
        public static T GetFromTarget<T>(ref T del, object target) where T : Delegate
        {
            if (del == null)
                return null;

            foreach (T d in del.GetInvocationList())
            {
                if (d.Target == target)
                    return d;
            }

            return null;
        }

        public static bool ReplaceFromTarget<T>(ref T del, object target, T newDelegate) where T : Delegate
        {
            T targetDelegate = GetFromTarget(ref del, target);

            if (targetDelegate != null)
            {
                del = (T)Delegate.Remove(del, targetDelegate);
                del = (T)Delegate.Combine(del, newDelegate);
                return true;
            }
            return false;
        }

        public static T RemoveFromTarget<T>(ref T del, object target) where T : Delegate
        {
            T targetDelegate = GetFromTarget(ref del, target);

            if (targetDelegate != null)
            {
                del = (T)Delegate.Remove(del, targetDelegate);
                return targetDelegate;
            }
            return null;
        }
    }
}
