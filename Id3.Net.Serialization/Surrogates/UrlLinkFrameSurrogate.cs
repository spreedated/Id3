﻿#region --- License & Copyright Notice ---
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
using System.Runtime.Serialization;

namespace Id3.Serialization.Surrogates
{
    internal sealed class UrlLinkFrameSurrogate : Id3FrameSurrogate<UrlLinkFrame>
    {
        protected override void GetFrameData(UrlLinkFrame frame, SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Url", frame.Url);
        }

        protected override UrlLinkFrame SetObjectData(UrlLinkFrame frame, SerializationInfo info, StreamingContext context,
            ISurrogateSelector selector)
        {
            frame.Url = info.GetString("Url");
            return frame;
        }
    }
}