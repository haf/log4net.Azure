<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="SampleAzureProj" generation="1" functional="0" release="0" Id="2eee0b5c-c586-4408-8f2b-9cffe7c55c07" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="SampleAzureProjGroup" generation="1" functional="0" release="0">
      <settings>
        <aCS name="log4net.Azure.Sample:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/SampleAzureProj/SampleAzureProjGroup/Maplog4net.Azure.Sample:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="log4net.Azure.SampleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/SampleAzureProj/SampleAzureProjGroup/Maplog4net.Azure.SampleInstances" />
          </maps>
        </aCS>
      </settings>
      <maps>
        <map name="Maplog4net.Azure.Sample:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/SampleAzureProj/SampleAzureProjGroup/log4net.Azure.Sample/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="Maplog4net.Azure.SampleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/SampleAzureProj/SampleAzureProjGroup/log4net.Azure.SampleInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="log4net.Azure.Sample" generation="1" functional="0" release="0" software="D:\code\oss\log4net.Azure\src\SampleAzureProj\csx\Debug\roles\log4net.Azure.Sample" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="1792" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;log4net.Azure.Sample&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;log4net.Azure.Sample&quot; /&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/SampleAzureProj/SampleAzureProjGroup/log4net.Azure.SampleInstances" />
            <sCSPolicyFaultDomainMoniker name="/SampleAzureProj/SampleAzureProjGroup/log4net.Azure.SampleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyFaultDomain name="log4net.Azure.SampleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="log4net.Azure.SampleInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
</serviceModel>