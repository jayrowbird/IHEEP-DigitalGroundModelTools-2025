//The MIT License (MIT)

//Copyright (c) 2014 Dave Kerr

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FileFormatWavefront
{
    /// <summary>
    /// Represents the results of a file loading operation.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    public class FileLoadResult<TModel>
    {
        private readonly TModel model;
        private readonly List<Message> messages;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLoadResult{TModel}"/> class.
        /// </summary>
        /// <param name="messages">The messages.</param>
        /// <param name="model">The model.</param>
        internal FileLoadResult(TModel model, List<Message> messages)
        {
            this.model = model;
            this.messages = messages;
        }

        /// <summary>
        /// Gets the loaded model.
        /// </summary>
        public TModel Model
        {
            get { return model; }
        }

        /// <summary>
        /// Gets the file load messages.
        /// </summary>
        public ReadOnlyCollection<Message> Messages
        {
            get { return messages.AsReadOnly(); }
        }
    }
}