using System;


namespace Common.Support.ErrorHandling
{
#if !SILVERLIGHT
    [Serializable]
    public class CMSExceptionDetail
    {
    }

#else
    [DataContract ]
    public class CMSExceptionDetail
    {
    }

#endif

}
