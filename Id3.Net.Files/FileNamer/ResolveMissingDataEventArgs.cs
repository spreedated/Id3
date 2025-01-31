#region --- License & Copyright Notice ---
/*
Copyright (c) 2005-2018 Jeevan James
All rights reserved.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

using Id3.Frames;
using System;

namespace Id3.Files
{
    public sealed class ResolveMissingDataEventArgs : EventArgs
    {
        internal ResolveMissingDataEventArgs(Id3Tag tag, Id3Frame frame, string sourceName)
        {
            Tag = tag;
            Frame = frame;
            SourceName = sourceName;
        }

        public Id3Tag Tag { get; }

        public Id3Frame Frame { get; }

        public string Value { get; set; }

        public string SourceName { get; }
    }
}