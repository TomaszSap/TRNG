﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.Utils;
using NAudio.Wave;


namespace RNG
{
    internal class Extractor : IWaveIn
    {

        const int SAMPLE_RATE = 44100;
        const int BUFFER_SIZE = 1 * 1024 * 1024 * 2;//1MB * 2(evenly numbers 0 and 255 are discarded)
        const int SAMPLE_ARRAY_SIZE = BUFFER_SIZE * 3;
        const int MILISTOWAIT = 20000;
        const int OFFSET = 80000;

        const byte pattern3LSB = 0b00000111;



        private WaveInEvent waveIn=new WaveInEvent();
        byte[] buffer= new byte[BUFFER_SIZE];
        MemoryStream memoryStream=new MemoryStream(BUFFER_SIZE);
        

        
        public WaveFormat WaveFormat { get; set; }

        public event EventHandler<WaveInEventArgs> DataAvailable;
        public event EventHandler<StoppedEventArgs> RecordingStopped;
        


        public Extractor()
        {
            
            DataAvailable = WaveIn_DataAvailable;
            RecordingStopped = WaveIn_RecordingStopped;

            this.WaveFormat = new WaveFormat(SAMPLE_RATE, WaveIn.GetCapabilities(0).Channels);
            this.waveIn.WaveFormat = this.WaveFormat;
            this.waveIn.DeviceNumber = 0;
            this.waveIn.DataAvailable += DataAvailable;
            this.waveIn.BufferMilliseconds = 1000;
            this.waveIn.RecordingStopped += RecordingStopped;


        }

        public async Task GetSamples()
        {

            StartRecording();

            Thread.Sleep(MILISTOWAIT);

            StopRecording();


            buffer = memoryStream.GetBuffer()[OFFSET..(BUFFER_SIZE + OFFSET)];
            
            await memoryStream.FlushAsync();
        }

        public void Parser()
        {
            buffer=buffer.Where((x, idx) => (long)idx%2!=1).ToArray();//getting buffer without extra 0 and 255
            var temp = buffer.Select(x => (x & pattern3LSB)!=0).ToArray();
            var arr = new bool[SAMPLE_ARRAY_SIZE];
            for(int i = 0; i < BUFFER_SIZE; i++)
            {
                arr[3 * i + 0] = (buffer[i] & 0b00000001) != 0;
                arr[3 * i + 0] = (buffer[i] & 0b00000010) != 0;
                arr[3 * i + 0] = (buffer[i] & 0b00000100) != 0;
            }

        }
        
        private void WaveIn_RecordingStopped(object? sender, StoppedEventArgs e)
        {
            //TODO
        }

        private void WaveIn_DataAvailable(object? sender, WaveInEventArgs e)
        {
            memoryStream.Write(e.Buffer, 0, e.BytesRecorded);
        }

        public void Dispose()
        {
            if(waveIn!=null)
                waveIn.Dispose();
            if (memoryStream != null)
            {
                memoryStream.Dispose();
            }
        }

        public void StartRecording()
        {
            this.waveIn.StartRecording();
        }

        public void StopRecording()
        {
            this.waveIn.StopRecording();
        }
        

        
    }
}
