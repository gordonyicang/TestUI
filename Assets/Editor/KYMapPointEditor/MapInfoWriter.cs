using System.IO; 
using System.Net;
using System;
 
namespace KYSystem
{
	public class MapInfoWriter
	{
		private BinaryWriter writer;
 
		public MapInfoWriter(FileStream fs)
		{
			this.writer = new BinaryWriter(fs);
		}
 
		public void Close()
		{
			this.writer.Close();
		}
 
		public void WriteByte(byte value)
		{
			this.writer.Write(value);
		} 
 
 
		public void WriteInt16(short value)
		{
            this.writer.Write(value);
			//this.writer.Write
			//(
			//	BitConverter.GetBytes
			//	(
			//		IPAddress.HostToNetworkOrder(value)
			//	)
			//);
		}
 
		public void WriteInt32(int value)
		{
            this.writer.Write(value);
            //this.writer.Write
			//(
			//	BitConverter.GetBytes
			//	(
			//		IPAddress.HostToNetworkOrder(value)
			//	)
			//);
		}
 
		public void WriteInt64(long value)
		{
            this.writer.Write(value);
            //this.writer.Write
			//(
			//	BitConverter.GetBytes
			//	(
			//		IPAddress.HostToNetworkOrder(value)
			//	)
			//);
		}
 
		public void WriteString8(string value)
		{
			WriteByte
			(
				(byte) value.Length
			);
 
 
			this.writer.Write
			(
				System.Text.Encoding.UTF8.GetBytes(value)
			);
		}
 
 
		public void WriteString16(string value)
		{
			WriteInt16
			(
				(short) value.Length
			);
			
			
			this.writer.Write
			(
				System.Text.Encoding.UTF8.GetBytes(value)
			);
		}

		public long GetPosition()
		{
			return this.writer.BaseStream.Position;
		}

		public void SetPosition(long pos)
		{
			this.writer.BaseStream.Position = pos;
		}
	}
}
