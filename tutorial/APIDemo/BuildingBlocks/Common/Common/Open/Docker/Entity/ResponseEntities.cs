////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 3/3/2015 10:15:10 AM 
// Description: DockerContainer.cs  
//https://docs.docker.com/reference/api/docker_remote_api_v1.16/#list-containers
// Revisions  :            		
// **************************************************************************** 
using System;

namespace Common.Open.Docker.Entity
{
    //**********************************************************
    public class DockerEventInformation
    {
        public string status { get; set; }
        public string id { get; set; }
        public string from { get; set; }
        public int time { get; set; }
    }

    //**********************************************************

    public class DockerVersionInformation
    {
        public string ApiVersion { get; set; }
        public string Arch { get; set; }
        public string GitCommit { get; set; }
        public string GoVersion { get; set; }
        public string KernelVersion { get; set; }
        public string Os { get; set; }
        public string Version { get; set; }
    }


    //**********************************************************
    public class SystemWideInformation
    {
        public int Containers { get; set; }
        public int Debug { get; set; }
        public string DockerRootDir { get; set; }
        public string Driver { get; set; }
        public string[][] DriverStatus { get; set; }
        public string ExecutionDriver { get; set; }
        public string ID { get; set; }
        public int IPv4Forwarding { get; set; }
        public int Images { get; set; }
        public string IndexServerAddress { get; set; }
        public string InitPath { get; set; }
        public string InitSha1 { get; set; }
        public string KernelVersion { get; set; }
        public object Labels { get; set; }
        public int MemTotal { get; set; }
        public int MemoryLimit { get; set; }
        public int NCPU { get; set; }
        public int NEventsListener { get; set; }
        public int NFd { get; set; }
        public int NGoroutines { get; set; }
        public string Name { get; set; }
        public string OperatingSystem { get; set; }
        public Registryconfig RegistryConfig { get; set; }
        public int SwapLimit { get; set; }
    }

    public class Registryconfig
    {
        public Indexconfigs IndexConfigs { get; set; }
        public string[] InsecureRegistryCIDRs { get; set; }
    }

    public class Indexconfigs
    {
        public DockerIo dockerio { get; set; }
    }

    public class DockerIo
    {
        public object Mirrors { get; set; }
        public string Name { get; set; }
        public bool Official { get; set; }
        public bool Secure { get; set; }
    }

    //**********************************************************
    public class HistoryOfImage
    {
        public string Id { get; set; }
        public int Created { get; set; }
        public string CreatedBy { get; set; }
    }
    //**********************************************************
    public class InspectImage
    {
        public DateTime Created { get; set; }
        public string Container { get; set; }
        public Containerconfig ContainerConfig { get; set; }
        public string Id { get; set; }
        public string Parent { get; set; }
        public int Size { get; set; }
    }

    public class Containerconfig
    {
        public string Hostname { get; set; }
        public string User { get; set; }
        public int Memory { get; set; }
        public int MemorySwap { get; set; }
        public bool AttachStdin { get; set; }
        public bool AttachStdout { get; set; }
        public bool AttachStderr { get; set; }
        public object PortSpecs { get; set; }
        public bool Tty { get; set; }
        public bool OpenStdin { get; set; }
        public bool StdinOnce { get; set; }
        public object Env { get; set; }
        public string[] Cmd { get; set; }
        public object Dns { get; set; }
        public string Image { get; set; }
        public object Volumes { get; set; }
        public string VolumesFrom { get; set; }
        public string WorkingDir { get; set; }
    }

    //**********************************************************
    public class ImageRlt
    {
        /// <summary>
        /// **
        /// </summary>
        public string Id { get; set; }
        public string status { get; set; }
        public string progress { get; set; }
        public string Untagged { get; set; }
        public string Deleted { get; set; }
        public Progressdetail progressDetail { get; set; }
        public string Stream { get; set; }
        public string error { get; set; }
    }

    public class Progressdetail
    {
        public int current { get; set; }
        public int total { get; set; }
    }

    //**********************************************************


    public class Image
    {
        public string[] RepoTags { get; set; }
        public string Id { get; set; }
        public int Created { get; set; }
        public int Size { get; set; }
        public int VirtualSize { get; set; }
        public string ParentId { get; set; }

        //-------------search rlt---------------------
        public string description { get; set; }
        public bool is_official { get; set; }
        public bool is_automated { get; set; }
        public string name { get; set; }
        public int star_count { get; set; }

    }


    //**********************************************************
    public class InspectChangesOnFileSystem
    {
        public string Path { get; set; }
        public int Kind { get; set; }
    }

    //**********************************************************
    public class ProcessesOfContainer
    {
        public string[] Titles { get; set; }
        public string[][] Processes { get; set; }
    }

    //**********************************************************

    public class InspectContainer
    {
        public string AppArmorProfile { get; set; }
        public object[] Args { get; set; }
        public Config Config { get; set; }
        public DateTime Created { get; set; }
        public string Driver { get; set; }
        public string ExecDriver { get; set; }
        public Hostconfig HostConfig { get; set; }
        public string HostnamePath { get; set; }
        public string HostsPath { get; set; }
        public string Id { get; set; }
        public string Image { get; set; }
        public string MountLabel { get; set; }
        public string Name { get; set; }
        public Networksettings NetworkSettings { get; set; }
        public string Path { get; set; }
        public string ProcessLabel { get; set; }
        public string ResolvConfPath { get; set; }
        public State State { get; set; }
        public Volumes Volumes { get; set; }
        public Volumesrw VolumesRW { get; set; }
    }

    public class Config
    {
        public bool AttachStderr { get; set; }
        public bool AttachStdin { get; set; }
        public bool AttachStdout { get; set; }
        public string[] Cmd { get; set; }
        public int CpuShares { get; set; }
        public string Cpuset { get; set; }
        public string Domainname { get; set; }
        public object Entrypoint { get; set; }
        public string[] Env { get; set; }
        public object ExposedPorts { get; set; }
        public string Hostname { get; set; }
        public string Image { get; set; }
        public string MacAddress { get; set; }
        public int Memory { get; set; }
        public int MemorySwap { get; set; }
        public bool NetworkDisabled { get; set; }
        public object OnBuild { get; set; }
        public bool OpenStdin { get; set; }
        public object PortSpecs { get; set; }
        public bool StdinOnce { get; set; }
        public bool Tty { get; set; }
        public string User { get; set; }
        public object Volumes { get; set; }
        public string WorkingDir { get; set; }
    }
    /// <summary>
    /// *******
    /// </summary>
    public class Hostconfig
    {
        public object Binds { get; set; }
        public object CapAdd { get; set; }
        public object CapDrop { get; set; }
        public string ContainerIDFile { get; set; }
        public object[] Devices { get; set; }
        public object Dns { get; set; }
        public object DnsSearch { get; set; }
        public object ExtraHosts { get; set; }
        public string IpcMode { get; set; }
        public object Links { get; set; }
        public object[] LxcConf { get; set; }
        public string NetworkMode { get; set; }
        public Portbindings PortBindings { get; set; }
        public bool Privileged { get; set; }
        public bool PublishAllPorts { get; set; }
        public Restartpolicy RestartPolicy { get; set; }
        public object SecurityOpt { get; set; }
        public object VolumesFrom { get; set; }

        //-----

    }

    //public class Restartpolicy
    //{
    //public int MaximumRetryCount { get; set; }
    //public string Name { get; set; }
    //}

    public class Networksettings
    {
        public string Bridge { get; set; }
        public string Gateway { get; set; }
        public string IPAddress { get; set; }
        public int IPPrefixLen { get; set; }
        public string MacAddress { get; set; }
        public object PortMapping { get; set; }
        public Ports Ports { get; set; }
    }

    public class Ports
    {
    }

    public class State
    {
        public string Error { get; set; }
        public int ExitCode { get; set; }
        public DateTime FinishedAt { get; set; }
        public bool OOMKilled { get; set; }
        public bool Paused { get; set; }
        public int Pid { get; set; }
        public bool Restarting { get; set; }
        public bool Running { get; set; }
        public DateTime StartedAt { get; set; }
    }


    public class Volumesrw
    {
    }



    /// <summary>
    /// $$$$$
    /// </summary>
    public class Portbindings
    {
        public _80Tcp[] _80tcp { get; set; }
        public _22Tcp1[] _22tcp { get; set; }
    }

    public class _80Tcp
    {
        public string HostIp { get; set; }
        public string HostPort { get; set; }
    }


    //***********************************************************
    public class ContainerArray
    {
        public Container[] Property1 { get; set; }
    }

    public class Container
    {
        public string Command { get; set; }
        public int Created { get; set; }
        public string Id { get; set; }
        public string Image { get; set; }
        public string[] Names { get; set; }
        public Port[] Ports { get; set; }
        public int SizeRootFs { get; set; }
        public int SizeRw { get; set; }
        public string Status { get; set; }
    }

    public class Port
    {
        public string IP { get; set; }
        public int PrivatePort { get; set; }
        public int PublicPort { get; set; }
        public string Type { get; set; }
    }

}
