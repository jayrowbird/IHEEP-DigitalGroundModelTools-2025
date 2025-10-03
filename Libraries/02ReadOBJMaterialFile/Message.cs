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

using System;

namespace FileFormatWavefront
{
    /// <summary>
    /// Represents a message of a specific severity relating to the loading of data from a file.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// The message type.
        /// </summary>
        private readonly MessageType messageType;

        /// <summary>
        /// The file name. May be null.
        /// </summary>
        private readonly string fileName;

        /// <summary>
        /// The line number. May be null.
        /// </summary>
        private readonly int? lineNumber;

        /// <summary>
        /// The actual message details.
        /// </summary>
        private readonly string details;

        /// <summary>
        /// The exception associated with the message, may be null.
        /// </summary>
        private readonly Exception? exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="details">The message details.</param>
        /// <param name="exception">The exception.</param>
        public Message(MessageType messageType, string fileName, int? lineNumber, string details, Exception? exception = null)
        {
            this.messageType = messageType;
            this.fileName = fileName;
            this.lineNumber = lineNumber;
            this.details = details;
            this.exception = exception;
        }

        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        public MessageType MessageType
        {
            get { return messageType; }
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        public string FileName
        {
            get { return fileName; }
        }

        /// <summary>
        /// Gets the line number.
        /// </summary>
        public int? LineNumber
        {
            get { return lineNumber; }
        }

        /// <summary>
        /// Gets the details.
        /// </summary>
        public string Details
        {
            get { return details; }
        }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        public Exception? Exception
        {
            get { return exception; }
        }
    }
}