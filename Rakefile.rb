require "rubygems"
require "bundler"
Bundler.setup
$: << './'

require 'albacore'
require 'rake/clean'
require 'semver'

require 'buildscripts/utils'
require 'buildscripts/paths'
require 'buildscripts/project_details'
require 'buildscripts/environment'

# to get the current version of the project, type 'SemVer.find.to_s' in this rake file.

desc 'generate the shared assembly info'
assemblyinfo :assemblyinfo => ["env:release"] do |asm|
  data = commit_data() #hash + date
  asm.product_name = asm.title = PROJECTS[:az][:title]
  asm.description = PROJECTS[:az][:description] + " #{data[0]} - #{data[1]}"
  asm.company_name = PROJECTS[:az][:company]
  # This is the version number used by framework during build and at runtime to locate, link and load the assemblies. When you add reference to any assembly in your project, it is this version number which gets embedded.
  asm.version = BUILD_VERSION
  # Assembly File Version : This is the version number given to file as in file system. It is displayed by Windows Explorer. Its never used by .NET framework or runtime for referencing.
  asm.file_version = BUILD_VERSION
  asm.custom_attributes :AssemblyInformationalVersion => "#{BUILD_VERSION}", # disposed as product version in explorer
    :AssemblyConfiguration => "#{CONFIGURATION}",
    :Guid => PROJECTS[:az][:guid]
  asm.com_visible = false
  asm.copyright = PROJECTS[:az][:copyright]
  asm.output_file = File.join(FOLDERS[:src], 'SharedAssemblyInfo.cs')
  asm.namespaces = "System", "System.Reflection", "System.Runtime.InteropServices", "System.Security"
end


desc "build sln file"
msbuild :msbuild do |msb|
  msb.solution   = FILES[:sln]
  msb.properties :Configuration => CONFIGURATION
  msb.targets    :Clean, :Build
end


task :az_output => [:msbuild] do
  target = File.join(FOLDERS[:binaries], PROJECTS[:az][:id])
  copy_files FOLDERS[:az][:out], "*.{xml,dll,pdb,config}", target
  CLEAN.include(target)
end

task :output => [:az_output]
task :nuspecs => [:az_nuspec]

desc "Create a nuspec for 'log4net.Azure'"
nuspec :az_nuspec do |nuspec|
  nuspec.id = "#{PROJECTS[:az][:nuget_key]}"
  nuspec.version = BUILD_VERSION
  nuspec.authors = "#{PROJECTS[:az][:authors]}"
  nuspec.description = "#{PROJECTS[:az][:description]}"
  nuspec.title = "#{PROJECTS[:az][:title]}"
  nuspec.projectUrl = 'http://github.com/haf/log4net.Azure'
  nuspec.language = "en-US"
  nuspec.licenseUrl = "http://www.apache.org/licenses/LICENSE-2.0"
  nuspec.requireLicenseAcceptance = "false"
  
  nuspec.output_file = FILES[:az][:nuspec]
  nuspec_copy(:az, "#{PROJECTS[:az][:id]}.{dll,pdb,xml}")
end

task :nugets => [:"env:release", :nuspecs, :az_nuget]

desc "nuget pack 'log4net.Azure'"
nugetpack :az_nuget do |nuget|
   nuget.command     = "#{COMMANDS[:nuget]}"
   nuget.nuspec      = "#{FILES[:az][:nuspec]}"
   # nuget.base_folder = "."
   nuget.output      = "#{FOLDERS[:nuget]}"
end

task :publish => [:"env:release", :az_nuget_push]

desc "publishes (pushes) the nuget package 'log4net.Azure'"
nugetpush :az_nuget_push do |nuget|
  nuget.command = "#{COMMANDS[:nuget]}"
  nuget.package = "#{File.join(FOLDERS[:nuget], PROJECTS[:az][:nuget_key] + "." + BUILD_VERSION + '.nupkg')}"
# nuget.apikey = "...."
 # nuget.source = URIS[:nuget_offical]
  nuget.create_only = false
end

task :default  => ["env:release", "assemblyinfo", "msbuild", "output", "nugets"]

task :copy_local do
  packages = Dir.glob 'build/nuget/*.nupkg'
  packages.each do |p|
    cp p, 'D:/packages'
  end
end