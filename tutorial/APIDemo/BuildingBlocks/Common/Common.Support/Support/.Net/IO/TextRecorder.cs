////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 11/9/2009 10:25:21 AM 
// Description: Recorder.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Text;
using System.IO;

namespace Support.Net.IO
{
    public class TextRecorder:IDisposable
    {       
        public TextRecorder(string filePath)
        {
            Init(filePath);
        }
        
        public void Flush()
        {
            if (Writer != null)
            {
                Writer.Flush();                     
            }
            if (RecordStream != null)
            {               
                RecordStream.Flush();
            }
        }

        public void Write(string recordText)
        {
            var length = RecordStream.Length;
            if (length == 0)
            {
                Writer.Write(HeadText);
            }
            else
            {
                RecordStream.Position = length - EndText.Length;
            }
            Writer.Write(recordText);
            Writer.Write(EndText);
        }

        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass of this type implements a finalizer.
            System.GC.SuppressFinalize(this);
        }
        private void Init(string filePath)
        {
            if (FilePath == filePath) return;
            FilePath = filePath;
            RecordStream = new FileStream(FilePath, FileMode.OpenOrCreate);
            Writer = new StreamWriter(RecordStream, Encoding.UTF8);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //TODO: Add resource.Dispose() logic here
                    Flush();
                    Writer.Close();
                    RecordStream.Close();
                    
                }
            }
            //resource = null;
            disposed = true;
        }       
        public string FilePath { get; private set; }
        public string HeadText { get; set; }
        public string EndText { get; set; }
        private bool disposed;
        private FileStream RecordStream { get; set; }
        private TextWriter Writer { get; set; }
    }
}
