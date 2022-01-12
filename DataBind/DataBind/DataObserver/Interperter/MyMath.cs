using System;
using System.Collections.Generic;
using System.Text;

namespace vm.MathExt
{
    using number = Double;
    public class Math
    {
        public static number Max(params number[] nums)
        {
            if (nums.Length <= 0)
            {
                throw new ArgumentException($"Max expect params count >= 1, accept {nums.Length}");
            }

            number max = nums[0];
            foreach (number n in nums)
            {
                max = System.Math.Max(max, n);
            }
            return max;
        }
        public static number Min(params number[] nums)
        {
            if (nums.Length <= 0)
            {
                throw new ArgumentException($"Max expect params count >= 1, accept {nums.Length}");
            }

            number max = nums[0];
            foreach (number n in nums)
            {
                max = System.Math.Min(max, n);
            }
            return max;
        }
    }
}
