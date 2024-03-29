﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quras.Compiler.JVM
{
    public partial class ModuleConverter
    {
        private void _ConvertStLoc(JavaMethod method, OpCode src, NeoMethod to, int pos)
        {
            //push d
            var c = _Convert1by1(Pure.VM.OpCode.DUPFROMALTSTACK, src, to);
            if (c.debugcode == null)
            {
                c.debugcode = "from StLoc -> 6 code";
                c.debugline = 0;
            }
            _InsertPush(pos, "", to);//add index

            _InsertPush(2, "", to);
            _Insert1(Pure.VM.OpCode.ROLL, "", to);
            _Insert1(Pure.VM.OpCode.SETITEM, "", to);
        }
        private void _ConvertLdLoc(JavaMethod method, OpCode src, NeoMethod to, int pos)
        {
            if (method.method.IsStatic == false && pos == 0)
            {//忽略非静态函数取this的操作
                return;
            }
            //push d
            var c = _Convert1by1(Pure.VM.OpCode.DUPFROMALTSTACK, src, to);
            if (c.debugcode == null)
            {
                c.debugcode = "from LdLoc -> 5 code";
                c.debugline = 0;
            }
            _InsertPush(pos, "", to);//add index


            //pick
            _Convert1by1(Pure.VM.OpCode.PICKITEM, null, to);
        }
        //private void _ConvertLdLocA(OpCode src, AntsMethod to, int pos)
        //{
        //    _ConvertPush(pos, src, to);
        //}
        private void _ConvertLdArg(OpCode src, NeoMethod to, int pos)
        {
            //push d
            var c = _Convert1by1(Pure.VM.OpCode.DEPTH, src, to);
            if (c.debugcode == null)
            {
                c.debugcode = "from LdArg -> 5 code";
                c.debugline = 0;
            }
            //push n
            _ConvertPush(pos, null, to);//翻转取参数顺序
            //_Convert1by1(Pure.VM.OpCode.PUSHDATA1, null, to, int2Pushdata1bytes(to.paramtypes.Count - 1 - pos));
            //d+n
            _Convert1by1(Pure.VM.OpCode.ADD, null, to);

            //push olddepth
            _Convert1by1(Pure.VM.OpCode.FROMALTSTACK, null, to);
            _Convert1by1(Pure.VM.OpCode.DUP, null, to);
            _Convert1by1(Pure.VM.OpCode.TOALTSTACK, null, to);
            //(d+n)-olddepth
            _Convert1by1(Pure.VM.OpCode.SUB, null, to);

            //pick
            _Convert1by1(Pure.VM.OpCode.PICK, null, to);
        }
        public bool IsNonCall(JavaMethod method)
        {
            if (method != null)
                if (method.method.Annotations != null)
                {

                    object[] op = method.method.Annotations[0] as object[];
                    if (op[1] as string == "Lorg/neo/smartcontract/framework/Nonemit;")
                    {
                        return true;
                    }
                }

            return false;
        }
        public bool IsOpCall(JavaMethod method, OpCode src, out string callname)
        {
            if (method != null)
                if (method.method.Annotations != null)
                {

                    object[] op = method.method.Annotations[0] as object[];
                    if (op[1] as string == "Lorg/neo/smartcontract/framework/OpCode;")
                    {
                        if (op[2] as string == "value")
                        {
                            var info = op[3] as object[];
                            callname = info[2] as string;
                            return true;
                        }


                    }
                }


            //m.Annotations

            callname = "";
            return false;
        }
        public bool IsSysCall(JavaMethod method, OpCode src, out string callname)
        {
            if (method != null)
                if (method.method.Annotations != null)
                {

                    object[] op = method.method.Annotations[0] as object[];
                    if (op[1] as string == "Lorg/neo/smartcontract/framework/Syscall;")
                    {
                        if (op[2] as string == "value")
                        {
                            var info = op[3] as string;
                            callname = info;
                            return true;
                        }


                    }
                }


            //m.Annotations

            callname = "";
            return false;
        }
        public bool IsAppCall(JavaMethod method, OpCode src, out byte[] callhash)
        {
            if (method != null)
                if (method.method.Annotations != null)
                {

                    object[] op = method.method.Annotations[0] as object[];
                    if (op[1] as string == "Lorg/neo/smartcontract/framework/Appcall;")
                    {
                        if (op[2] as string == "value")
                        {
                            var info = op[3] as string;
                            if (info.Length < 40)
                            {
                                throw new Exception("appcall hash is too short.");
                            }
                            byte[] bytes = new byte[20];

                            for (var i = 0; i < 20; i++)
                            {
                                bytes[i] = byte.Parse(info.Substring(i * 2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                            }
                            //string hexhash 需要反序
                            callhash = bytes.Reverse().ToArray();
                            return true;
                        }


                    }
                }


            //m.Annotations

            callhash = null;
            return false;
        }
        private int _ConvertCall(JavaMethod method, OpCode src, NeoMethod to)
        {
            _Convert1by1(Pure.VM.OpCode.NOP, src, to);
            var cc = method.DeclaringType.classfile.constantpool;
            var c = cc[src.arg1] as javaloader.ClassFile.ConstantPoolItemMethodref;
            var name = c.Class + "::" + c.Name;

            List<string> paramTypes = new List<string>();
            string returntype;
            JavaMethod.scanTypes(c.Signature, out returntype, paramTypes);


            JavaClass javaclass = null;
            JavaMethod _javamethod = null;

            if (this.srcModule.classes.ContainsKey(c.Class))
            {
                javaclass = this.srcModule.classes[c.Class];
                if (javaclass.methods.ContainsKey(c.Name + c.Signature))
                {
                    _javamethod = javaclass.methods[c.Name + c.Signature];
                }
                else
                {
                    while (javaclass != null)
                    {
                        if (this.srcModule.classes.ContainsKey(javaclass.superClass))
                        {
                            javaclass = this.srcModule.classes[javaclass.superClass];
                            if (javaclass.methods.ContainsKey(c.Name + c.Signature))
                            {
                                _javamethod = javaclass.methods[c.Name + c.Signature];
                                break;
                            }
                        }
                        else
                        {
                            javaclass = null;
                        }
                    }
                }
            }
            int calltype = 0;
            string callname = "";
            byte[] callhash = null;
            Pure.VM.OpCode callcode = Pure.VM.OpCode.NOP;
            if (IsNonCall(_javamethod))
            {
                return 0;
            }
            else if (IsOpCall(_javamethod, src, out callname))
            {
                if (System.Enum.TryParse<Pure.VM.OpCode>(callname, out callcode))
                {
                    calltype = 2;
                }
                else
                {
                    throw new Exception("Can not find OpCall:" + callname);
                }
            }
            else if (IsSysCall(_javamethod, src, out callname))
            {
                calltype = 3;
            }
            else if (IsAppCall(_javamethod, src, out callhash))
            {
                calltype = 4;
            }
            else if (this.outModule.mapMethods.ContainsKey(c.Class + "::" + c.Name + c.Signature))
            {//this is a call
                calltype = 1;
            }
            else
            {

                if (name == "java.io.PrintStream::println")
                {//drop 1;
                    Console.WriteLine("logstr.");
                    _Convert1by1(Pure.VM.OpCode.DROP, src, to);
                    return 0;
                }
                else if (name == "java.math.BigInteger::<init>")
                {//do nothing
                    if (c.Signature == "([B)V")
                    {
                        return 0;
                    }
                    else if (c.Signature == "(Ljava/lang/String;)V")
                    {
                        throw new Exception("not support new BigInteger(string)");
                    }
                }
                else if (name == "java.math.BigInteger::add")
                {
                    _Convert1by1(Pure.VM.OpCode.ADD, src, to);
                    return 0;
                }
                else if (name == "java.math.BigInteger::subtract")
                {
                    _Convert1by1(Pure.VM.OpCode.SUB, src, to);
                    return 0;
                }
                else if (name == "java.math.BigInteger::multiply")
                {
                    _Convert1by1(Pure.VM.OpCode.MUL, src, to);
                    return 0;
                }
                else if (name == "java.math.BigInteger::divide")
                {
                    _Convert1by1(Pure.VM.OpCode.DIV, src, to);
                    return 0;
                }
                else if (name == "java.math.BigInteger::mod")
                {
                    _Convert1by1(Pure.VM.OpCode.MOD, src, to);
                    return 0;
                }
                else if (name == "java.math.BigInteger::compareTo")
                {
                    //need parse
                    _Convert1by1(Pure.VM.OpCode.SUB, src, to);
                    _Convert1by1(Pure.VM.OpCode.SIGN, null, to);
                    //_Convert1by1(Pure.VM.OpCode.DEC, src, to);
                    return 0;
                }
                // todo: what about java.lang.String::contentEquals?
                else if (name == "java.math.BigInteger::equals" ||
                    name == "java.lang.String::equals" ||
                    name == "kotlin.jvm.internal.Intrinsics::areEqual")
                {
                    _Convert1by1(Pure.VM.OpCode.NUMEQUAL, src, to);
                    //_Convert1by1(Pure.VM.OpCode.DEC, src, to);
                    return 0;
                }
                else if (name == "java.math.BigInteger::valueOf" ||
                    name == "java.math.BigInteger::intValue" ||
                    name == "java.lang.Boolean::valueOf" ||
                    name == "java.lang.Character::valueOf" ||
                    name == "java.lang.String::valueOf" ||
                    name == "java.lang.Long::valueOf" ||
                    name == "java.lang.Integer::valueOf" ||
                    name == "java.lang.Byte::valueOf" ||
                    name == "java.math.BigInteger::toByteArray")
                {
                    //donothing
                    return 0;
                }
                else if (name == "java.lang.Boolean::booleanValue" ||
                    name == "java.lang.Integer::integerValue" ||
                    name == "java.lang.Long::longValue" ||
                    name == "java.math.BigInteger::longValue")
                {
                    _Convert1by1(Pure.VM.OpCode.NOP, src, to);
                    return 0;
                }
                else if (name == "java.lang.String::hashCode")
                {
                    //java switch 的编译方式很奇怪
                    return 0;
                }
                else if (name == "java.lang.String::charAt")
                {
                    _ConvertPush(1, src, to);
                    _Convert1by1(Pure.VM.OpCode.SUBSTR, null, to);
                    return 0;
                }
                else if (name == "java.lang.String::length")
                {
                    _Convert1by1(Pure.VM.OpCode.SIZE, null, to);
                    return 0;
                }
                else if (c.Class == "java.lang.StringBuilder")
                {
                    return _ConvertStringBuilder(c.Name, null, to);
                }
                else if (name == "java.util.Arrays::equals" ||
                    name == "kotlin.jvm.internal.Intrinsics::areEqual")
                {
                    _Convert1by1(Pure.VM.OpCode.EQUAL, null, to);
                    return 0;
                }
                else if (name == "kotlin.jvm.internal.Intrinsics::checkParameterIsNotNull")
                {
                    _Convert1by1(Pure.VM.OpCode.DROP, null, to);
                    _Convert1by1(Pure.VM.OpCode.DROP, null, to);
                    return 0;
                }
                else if (name == "kotlin.jvm.internal.Intrinsics::throwNpe")
                {
                    _Convert1by1(Pure.VM.OpCode.THROW, src, to);
                    return 0;
                }
            }

            if (calltype == 0)
            {
                throw new Exception("unknown call:" + name);
            }
            var pcount = paramTypes.Count;

            if (calltype == 2)
            {
                //opcode call 
            }
            else
            {//翻转参数入栈顺序
                _Convert1by1(Pure.VM.OpCode.NOP, src, to);
                if (pcount <= 1)
                {

                }
                else if (pcount == 2)
                {
                    _Insert1(Pure.VM.OpCode.SWAP, "swap 2 param", to);
                }
                else if (pcount == 3)
                {
                    _InsertPush(2, "swap 0 and 2 param", to);
                    _Insert1(Pure.VM.OpCode.XSWAP, "", to);
                }
                else
                {
                    for (var i = 0; i < pcount / 2; i++)
                    {
                        int saveto = (pcount - 1 - i);
                        _InsertPush(saveto, "load" + saveto, to);
                        _Insert1(Pure.VM.OpCode.PICK, "", to);

                        _InsertPush(i + 1, "load" + i + 1, to);
                        _Insert1(Pure.VM.OpCode.PICK, "", to);


                        _InsertPush(saveto + 2, "save to" + saveto + 2, to);
                        _Insert1(Pure.VM.OpCode.XSWAP, "", to);
                        _Insert1(Pure.VM.OpCode.DROP, "", to);

                        _InsertPush(i + 1, "save to" + i + 1, to);
                        _Insert1(Pure.VM.OpCode.XSWAP, "", to);
                        _Insert1(Pure.VM.OpCode.DROP, "", to);

                    }
                }
            }
            if (calltype == 1)
            {
                var _c = _Convert1by1(Pure.VM.OpCode.CALL, null, to, new byte[] { 5, 0 });
                _c.needfixfunc = true;
                _c.srcfunc = name + c.Signature;
                return 0;
            }
            else if (calltype == 2)
            {
                _Convert1by1(callcode, null, to);
                return 0;
            }
            else if (calltype == 3)
            {
                var bytes = Encoding.UTF8.GetBytes(callname);
                if (bytes.Length > 252) throw new Exception("string is too long");
                byte[] outbytes = new byte[bytes.Length + 1];
                outbytes[0] = (byte)bytes.Length;
                Array.Copy(bytes, 0, outbytes, 1, bytes.Length);
                //bytes.Prepend 函数在 dotnet framework 4.6 编译不过
                _Convert1by1(Pure.VM.OpCode.SYSCALL, null, to, outbytes);
                return 0;
            }
            else if (calltype == 4)
            {
                _Convert1by1(Pure.VM.OpCode.APPCALL, null, to, callhash);

            }

            return 0;
        }

        private int _ConvertNewArray(JavaMethod method, OpCode src, NeoMethod to)
        {
            int skipcount = 0;
            if (src.arg1 != 8)
            {
                //this.logger.Log("_ConvertNewArray::not support type " + src.arg1 + " for array.");
                _Convert1by1(Pure.VM.OpCode.NEWARRAY, src, to);
                return 0;
            }
            //bytearray
            var code = to.body_Codes.Last().Value;
            //we need a number
            if (code.code > Pure.VM.OpCode.PUSH16)
            {
                throw new Exception("_ConvertNewArr::not support var lens for new byte[?].");
            }
            var number = getNumber(code);

            //移除上一条指令
            to.body_Codes.Remove(code.addr);
            this.addr = code.addr;

            OpCode next = src;
            int dupcount = 0;
            int pcount = 0;
            int[] buf = new int[] { 0, 0, 0 };
            byte[] outbuf = new byte[number];
            do
            {
                int n = method.GetNextCodeAddr(next.addr);
                next = method.body_Codes[n];
                if (next.code == javaloader.NormalizedByteCode.__invokestatic)
                {
                    var i = method.DeclaringType.classfile.constantpool[next.arg1] as javaloader.ClassFile.ConstantPoolItemMethodref;
                    var callname = i.Class + "::" + i.Name;
                    if (callname == "java.lang.Integer::valueOf")
                    {
                        //nothing
                        skipcount++;
                    }
                    else
                    {
                        throw new Exception("can not parse this new array code chain." + next.code);
                    }
                }
                else if (next.code == javaloader.NormalizedByteCode.__invokevirtual)
                {
                    var i = method.DeclaringType.classfile.constantpool[next.arg1] as javaloader.ClassFile.ConstantPoolItemMethodref;
                    var callname = i.Class + "::" + i.Name;
                    if (callname == "java.lang.Byte::byteValue")
                    {
                        skipcount++;
                    }
                    else
                    {
                        throw new Exception("can not parse this new array code chain." + next.code);
                    }
                }
                else if (next.code == javaloader.NormalizedByteCode.__checkcast)
                {
                    //nothing
                    skipcount++;
                }
                else if (next.code == javaloader.NormalizedByteCode.__dup)
                {
                    dupcount++;
                    skipcount++;
                }
                else if (next.code == javaloader.NormalizedByteCode.__iconst)
                {
                    buf[pcount] = next.arg1;
                    pcount++;
                    skipcount++;
                }
                else if (next.code == javaloader.NormalizedByteCode.__bastore)
                {
                    dupcount--;
                    var v = (byte)buf[pcount - 1];
                    var i = buf[pcount - 2];
                    //while (outbuf.Count <= i)
                    //    outbuf.Add(0);
                    outbuf[i] = v;
                    pcount -= 2;
                    skipcount++;
                }
                else if (next.code == javaloader.NormalizedByteCode.__astore)
                {
                    _ConvertPush(outbuf.ToArray(), src, to);
                    return skipcount;
                }
                else
                {
                    throw new Exception("can not parse this new array code chain.");
                }
            }
            while (next != null);

            return 0;
        }
        private int _ConvertNew(JavaMethod method, OpCode src, NeoMethod to)
        {
            var c = method.DeclaringType.classfile.constantpool[src.arg1] as javaloader.ClassFile.ConstantPoolItemClass;
            if (c.Name == "java.lang.StringBuilder")
            {
                _ConvertPush(1, src, to);
                _Insert1(Pure.VM.OpCode.NEWARRAY, "", to);
            }
            else if (c.Name == "java.math.BigInteger")
            {
                var next = method.GetNextCodeAddr(src.addr);
                if (method.body_Codes[next].code == javaloader.NormalizedByteCode.__dup)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                throw new Exception("new not supported type." + c.Name);
            }
            return 0;
        }
        private int _ConvertIfNonNull(JavaMethod method, OpCode src, NeoMethod to)
        {
            int nm = method.GetLastCodeAddr(src.addr);//上一指令
            int n = method.GetNextCodeAddr(src.addr);
            int n2 = method.GetNextCodeAddr(n);
            var codenext = method.body_Codes[n];

            if (nm >= 0 && n >= 0 && n2 >= 0
                && method.body_Codes[nm].code == javaloader.NormalizedByteCode.__dup //上一条是dup指令
                && src.arg1 == n2 - src.addr //刚好跳过throw 指令
                && codenext.code == javaloader.NormalizedByteCode.__invokestatic
                )
            {
                var cc = method.DeclaringType.classfile.constantpool;
                var c = cc[codenext.arg1] as javaloader.ClassFile.ConstantPoolItemMethodref;
                var name = c.Class + "::" + c.Name;
                if (name == "kotlin.jvm.internal.Intrinsics::throwNpe")
                {//识别到套路
                    var _code = to.body_Codes.Last().Value;
                    //移除上一条指令
                    to.body_Codes.Remove(_code.addr);
                    this.addr = _code.addr;

                    return 1;
                }
            }
            var codenextnext = method.body_Codes[n2];
            _ConvertPush(0, src, to);//和0比较
            _Convert1by1(Pure.VM.OpCode.NUMNOTEQUAL, null, to);
            var code = _Convert1by1(Pure.VM.OpCode.JMPIF, null, to, new byte[] { 0, 0 });
            code.needfix = true;
            code.srcaddr = src.addr + src.arg1;
            return 0;
        }
        private int _ConvertStringBuilder(string callname, OpCode src, NeoMethod to)
        {
            if (callname == "<init>")
            {
                _Convert1by1(Pure.VM.OpCode.SWAP, null, to);
                _Convert1by1(Pure.VM.OpCode.DUP, null, to);

                _ConvertPush(0, null, to);
                _ConvertPush(3, null, to);
                _Convert1by1(Pure.VM.OpCode.ROLL, null, to);
                _Convert1by1(Pure.VM.OpCode.SETITEM, null, to);
                return 0;
            }
            if (callname == "append")
            {
                _Convert1by1(Pure.VM.OpCode.SWAP, null, to);//把对象数组换上来
                _Convert1by1(Pure.VM.OpCode.DUP, null, to);
                _ConvertPush(0, null, to);
                _Convert1by1(Pure.VM.OpCode.PICKITEM, null, to);

                _ConvertPush(2, null, to);
                _Convert1by1(Pure.VM.OpCode.ROLL, null, to);
                _Convert1by1(Pure.VM.OpCode.SWAP, null, to);//把对象数组换上来
                _Convert1by1(Pure.VM.OpCode.CAT, null, to);

                _ConvertPush(0, null, to);
                _Convert1by1(Pure.VM.OpCode.SWAP, null, to);//把对象数组换上来
                _Convert1by1(Pure.VM.OpCode.SETITEM, null, to);
                return 0;
            }
            if (callname == "toString")
            {
                _ConvertPush(0, null, to);
                _Convert1by1(Pure.VM.OpCode.PICKITEM, null, to);
                return 0;
            }
            return 0;
        }
        //private int _ConvertNewArr(ILMethod method, OpCode src, AntsMethod to)
        //{
        //    var code = to.body_Codes.Last().Value;
        //    //we need a number
        //    if (code.code > Pure.VM.OpCode.PUSH16)
        //    {
        //        this.logger.Log("_ConvertNewArr::not support var lens for array.");
        //        return 0;
        //    }
        //    var number = getNumber(code);

        //    //移除上一条指令
        //    to.body_Codes.Remove(code.addr);
        //    this.addr--;
        //    if (code.bytes != null)
        //        this.addr -= code.bytes.Length;

        //    var type = src.tokenType;
        //    if (type != "System.Byte")
        //    {
        //        this.logger.Log("_ConvertNewArr::not support type " + type + " for array.");
        //    }
        //    else
        //    {
        //        int n = method.GetNextCodeAddr(src.addr);
        //        int n2 = method.GetNextCodeAddr(n);
        //        int n3 = method.GetNextCodeAddr(n2);
        //        if (method.body_Codes[n].code == CodeEx.Dup && method.body_Codes[n2].code == CodeEx.Ldtoken && method.body_Codes[n3].code == CodeEx.Call)
        //        {//這是在初始化數組

        //            var data = method.body_Codes[n2].tokenUnknown as byte[];
        //            this._ConvertPush(data, src, to);

        //            return 3;

        //        }
        //        else
        //        {
        //            this._ConvertPush(new byte[number], src, to);
        //        }
        //    }



        //    return 0;

        //}
        //private int _ConvertInitObj(OpCode src, AntsMethod to)
        //{
        //    var type = (src.tokenUnknown as Mono.Cecil.TypeReference).Resolve();
        //    _Convert1by1(Pure.VM.OpCode.NOP, src, to);//空白
        //    _ConvertPush(type.Fields.Count, null, to);//插入个数量
        //    _Insert1(Pure.VM.OpCode.ARRAYNEW, null, to);
        //    //然後要將計算棧上的第一個值，寫入第二個值對應的pos
        //    _Convert1by1(Pure.VM.OpCode.SWAP, null, to);//replace n to top

        //    //push d
        //    _Convert1by1(Pure.VM.OpCode.DEPTH, null, to);

        //    _Convert1by1(Pure.VM.OpCode.DEC, null, to);//d 多了一位，剪掉
        //    _Convert1by1(Pure.VM.OpCode.SWAP, null, to);//把n拿上來
        //    //push n
        //    //_ConvertPush(pos, null, to);有n了
        //    //d-n-1
        //    _Convert1by1(Pure.VM.OpCode.SUB, null, to);
        //    _Convert1by1(Pure.VM.OpCode.DEC, null, to);

        //    //push olddepth
        //    _Convert1by1(Pure.VM.OpCode.FROMALTSTACK, null, to);
        //    _Convert1by1(Pure.VM.OpCode.DUP, null, to);
        //    _Convert1by1(Pure.VM.OpCode.TOALTSTACK, null, to);
        //    //(d-n-1)-olddepth
        //    _Convert1by1(Pure.VM.OpCode.SUB, null, to);

        //    //swap d-n-1 and top
        //    _Convert1by1(Pure.VM.OpCode.XSWAP, null, to);
        //    //drop top
        //    _Convert1by1(Pure.VM.OpCode.DROP, null, to);
        //    return 0;
        //}
        //private int _ConvertStfld(OpCode src, AntsMethod to)
        //{
        //    var field = (src.tokenUnknown as Mono.Cecil.FieldReference).Resolve();
        //    var type = field.DeclaringType;
        //    var id = type.Fields.IndexOf(field);
        //    if (id < 0)
        //        throw new Exception("impossible.");
        //    _Convert1by1(Pure.VM.OpCode.NOP, src, to);//空白

        //    _Convert1by1(Pure.VM.OpCode.SWAP, null, to);//把n拿上來 n 和 item
        //    //push d
        //    _Convert1by1(Pure.VM.OpCode.DEPTH, src, to);
        //    _Convert1by1(Pure.VM.OpCode.DEC, null, to);//d 多了一位，剪掉
        //    _Convert1by1(Pure.VM.OpCode.SWAP, null, to);//把n拿上來

        //    //push n
        //    //_ConvertPush(pos, null, to);有n了
        //    //d-n-1
        //    _Convert1by1(Pure.VM.OpCode.SUB, null, to);
        //    _Convert1by1(Pure.VM.OpCode.DEC, null, to);

        //    //push olddepth
        //    _Convert1by1(Pure.VM.OpCode.FROMALTSTACK, null, to);
        //    _Convert1by1(Pure.VM.OpCode.DUP, null, to);
        //    _Convert1by1(Pure.VM.OpCode.TOALTSTACK, null, to);
        //    //(d-n-1)-olddepth
        //    _Convert1by1(Pure.VM.OpCode.SUB, null, to);

        //    //pick
        //    _Convert1by1(Pure.VM.OpCode.PICK, null, to);


        //    _Convert1by1(Pure.VM.OpCode.SWAP, null, to);//把item 拿上來 
        //    _ConvertPush(id, null, to);
        //    _Convert1by1(Pure.VM.OpCode.ARRAYSETITEM, null, to);//修改值
        //    return 0;
        //}

        //private int _ConvertLdfld(OpCode src, AntsMethod to)
        //{
        //    var field = (src.tokenUnknown as Mono.Cecil.FieldReference).Resolve();
        //    var type = field.DeclaringType;
        //    var id = type.Fields.IndexOf(field);
        //    if (id < 0)
        //        throw new Exception("impossible.");
        //    _ConvertPush(id, src, to);
        //    _Convert1by1(Pure.VM.OpCode.PICKITEM, null, to);//修改值

        //    return 0;
        //}
    }

}
