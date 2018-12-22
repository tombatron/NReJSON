namespace NReJSON
{
    internal class CommandType
    {
        internal enum Json
        {
            DEL,
            GET,
            MGET,
            SET,
            TYPE,
            NUMINCRBY,
            NUMMULTBY,
            STRAPPEND,
            STRLEN,
            ARRAPPEND,
            ARRINSERT,
            ARRLEN,
            ARRPOP,
            ARRTRIM,
            OBJKEYS,
            OBJLEN,
            DEBUG,
            FORGET,
            RESP
        }
    }
}