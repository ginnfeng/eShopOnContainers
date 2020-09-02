////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 3/3/2015 10:30:47 AM 
// Description: RequestEntities.cs  
// Revisions  :            		
// **************************************************************************** 

namespace Common.Open.Docker.Entity
{
    //*******************************************
    public class DockerEventParameter
    {
    public string Since { get; set; }
    public string Until { get; set; }
    public string Filters { get; set; }
    public string Event { get; set; }
    public string Image { get; set; }
    public string Container { get; set; }
    }

    //*******************************************
    public class AuthArg
    {
    public string username { get; set; }
    public string password { get; set; }
    public string email { get; set; }
    public string serveraddress { get; set; }
    }
    //*******************************************
    public class CopyFromContainerArg
    {
        public string Resource { get; set; }
    }
    //*******************************************
    public class ContainerRlt
    {
        public string Id { get; set; }
        public object[] Warnings { get; set; }
    }
    public class ContainerArg
    {
        public string Hostname { get; set; }
        public string Domainname { get; set; }
        public string User { get; set; }
        public int Memory { get; set; }
        public int MemorySwap { get; set; }
        public int CpuShares { get; set; }
        public string Cpuset { get; set; }
        public bool AttachStdin { get; set; }
        public bool AttachStdout { get; set; }
        public bool AttachStderr { get; set; }
        public bool Tty { get; set; }
        public bool OpenStdin { get; set; }
        public bool StdinOnce { get; set; }
        public object Env { get; set; }
        public string[] Cmd { get; set; }
        public string Entrypoint { get; set; }
        public string Image { get; set; }
        public Volumes Volumes { get; set; }
        public string WorkingDir { get; set; }
        public bool NetworkDisabled { get; set; }
        public string MacAddress { get; set; }
        public Exposedports ExposedPorts { get; set; }
        public string[] SecurityOpts { get; set; }
        public Hostconfig HostConfig { get; set; }
    }

    public class Volumes
    {
        public Tmp tmp { get; set; }
    }

    public class Tmp
    {
    }

    public class Exposedports
    {
        public _22Tcp _22tcp { get; set; }
    }

    public class _22Tcp
    {
    }

   
    public class Lxcconf
    {
        public string lxcutsname { get; set; }
    }

    
    public class _22Tcp1
    {
        public string HostPort { get; set; }
    }

    public class Restartpolicy
    {
        public string Name { get; set; }
        public int MaximumRetryCount { get; set; }
    }


}
