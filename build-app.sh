#!/bin/bash
dotnet restore -r osx-x64
dotnet msbuild -t:BundleApp -p:RuntimeIdentifier=osx-x64 -property:Configuration=Release -p:UseAppHost=true -p:CFBundleDisplayName=MahiruLauncher -p:CFBundleShortVersionString=$1
