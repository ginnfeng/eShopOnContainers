#region Copyright (C) 2007 IgorM

/* 
 *	Copyright (C) 2005-2007 Igor Moochnick
 *	http://igorshare.wordpress.com
 *
 *  This Program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2, or (at your option)
 *  any later version.
 *   
 *  This Program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU General Public License for more details.
 *   
 *  You should have received a copy of the GNU General Public License
 *  along with GNU Make; see the file COPYING.  If not, write to
 *  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA. 
 *  http://www.gnu.org/copyleft/gpl.html
 * 
 *  The code originally developed by IgorM (C) 2007
 *
 */

#endregion

using System;

namespace Support.Net.CodeDom
{
    /// <summary>
    /// Converts from "object" general type to a concrete type
    /// </summary>
    /// <remarks>
    /// The class is working with .Net 2.0 and above
    /// </remarks>
    public class Converter
    {
        /// <summary>
        /// Returns True if the type can get Null as a value (is a reference type and not a value one)
        /// </summary>
        public static bool IsNullable(Type t)
        {
            if (!t.IsGenericType) return false;
            Type g = t.GetGenericTypeDefinition();
            return (g.Equals(typeof(Nullable<>)));
        }

        /// <summary>
        /// Returns a real type of a first generic argument
        /// </summary>
        private static Type UnderlyingTypeOf(Type t)
        {
            return t.GetGenericArguments()[0];
        }

        /// <summary>
        /// Converter
        /// </summary>
        public static T To<T>(object value, T defaultValue)
        {
            if (value == DBNull.Value) return defaultValue;
            Type t = typeof(T);
            if (IsNullable(t))
            {
                if (value == null) return default(T);
                t = UnderlyingTypeOf(t);
            }
            else
            {
                if ((value == null) && (t.IsValueType))
                {
                    return defaultValue;
                }
            }

            return (T)Convert.ChangeType(value, t);
        }
    }
}
