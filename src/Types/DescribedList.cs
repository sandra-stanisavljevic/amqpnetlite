﻿//  ------------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation
//  All rights reserved. 
//  
//  Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this 
//  file except in compliance with the License. You may obtain a copy of the License at 
//  http://www.apache.org/licenses/LICENSE-2.0  
//  
//  THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
//  EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR 
//  CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR 
//  NON-INFRINGEMENT. 
// 
//  See the Apache Version 2.0 License for specific language governing permissions and 
//  limitations under the License.
//  ------------------------------------------------------------------------------------

namespace Amqp.Types
{
    using System;

    public abstract class DescribedList : RestrictedDescribed
    {
        readonly object[] fields;

        protected DescribedList(Descriptor descriptor, int fieldCount)
            : base(descriptor)
        {
            this.fields = new object[fieldCount];
        }

        protected object[] Fields
        {
            get { return this.fields; }
        }

        internal override void DecodeValue(ByteBuffer buffer)
        {
            var list = Encoder.ReadList(buffer, FormatCode.Unknown);
            int count = list.Count < this.fields.Length ? list.Count : this.fields.Length;
            for (int i = 0; i < count; i++)
            {
                this.fields[i] = list[i];
            }
        }

        internal override void EncodeValue(ByteBuffer buffer)
        {
            Encoder.WriteList(buffer, this.fields);
        }

#if TRACE
        protected string GetDebugString(string name, object[] fieldNames, object[] fieldValues)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(name);
            sb.Append("(");
            bool addComma = false;
            for (int i = 0; i < fieldValues.Length; i++)
            {
                if (fieldValues[i] != null)
                {
                    if (addComma)
                    {
                        sb.Append(",");
                    }

                    sb.Append(fieldNames[i]);
                    sb.Append(":");
                    sb.Append(GetStringObject(fieldValues[i]));
                    addComma = true;
                }
            }
            sb.Append(")");

            return sb.ToString();
        }

        object GetStringObject(object value)
        {
            byte[] binary = value as byte[];
            if (binary != null)
            {
                string hexChars = "0123456789ABCDEF";
                System.Text.StringBuilder sb = new System.Text.StringBuilder(binary.Length * 2);
                for (int i = 0; i < binary.Length; ++i)
                {
                    sb.Append(hexChars[binary[i] >> 4]);
                    sb.Append(hexChars[binary[i] & 0x0F]);
                }

                return sb.ToString();
            }

            object[] list = value as object[];
            if (list != null)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append('[');
                for (int i = 0; i < list.Length; ++i)
                {
                    if (i > 0) sb.Append(',');
                    sb.Append(list[i]);
                }
                sb.Append(']');

                return sb.ToString();
            }

            return value;
        }
#endif
    }
}
