using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corp.TestTcpClient
{
  class PosMessageGenerator : IMessageGenerator
  {
    int counter = 0;
    string requestTemplate = "PHRQ123456789012345678901234567890123456{0}";
    public byte[] GenerateTransactionMessage()
    {

      byte[] request = new byte[7+189];
      request[7+0] = (byte)255;
      request[7+1] = (byte)83;
      request[7+2] = (byte)84;
      request[7+3] = (byte)76;
      request[7+4] = (byte)77;
      request[7+5] = (byte)48;
      request[7+6] = (byte)49;
      request[7+7] = (byte)63;
      request[7+8] = (byte)65;
      request[7+9] = (byte)255;
      request[7+10] = (byte)33;
      request[7+11] = (byte)84;
      request[7+12] = (byte)79;
      request[7+13] = (byte)82;
      request[7+14] = (byte)81;
      request[7+15] = (byte)63;
      request[7+16] = (byte)66;
      request[7+17] = (byte)33;
      request[7+18] = (byte)255;
      request[7+19] = (byte)55;
      request[7+20] = (byte)54;
      request[7+21] = (byte)49;
      request[7+22] = (byte)48;
      request[7+23] = (byte)55;
      request[7+24] = (byte)49;
      request[7+25] = (byte)55;
      request[7+26] = (byte)55;
      request[7+27] = (byte)159;
      request[7+28] = (byte)32;
      request[7+29] = (byte)32;
      request[7+30] = (byte)63;
      request[7+31] = (byte)67;
      request[7+32] = (byte)33;
      request[7+33] = (byte)0;
      request[7+34] = (byte)5;
      request[7+35] = (byte)6;
      request[7+36] = (byte)0;
      request[7+37] = (byte)68;
      request[7+38] = (byte)255;
      request[7+39] = (byte)33;
      request[7+40] = (byte)49;
      request[7+41] = (byte)52;
      request[7+42] = (byte)48;
      request[7+43] = (byte)56;
      request[7+44] = (byte)50;
      request[7+45] = (byte)55;
      request[7+46] = (byte)63;
      request[7+47] = (byte)253;
      request[7+48] = (byte)69;
      request[7+49] = (byte)23;
      request[7+50] = (byte)0;
      request[7+51] = (byte)49;
      request[7+52] = (byte)57;
      request[7+53] = (byte)48;
      request[7+54] = (byte)53;
      request[7+55] = (byte)63;
      request[7+56] = (byte)70;
      request[7+57] = (byte)235;
      request[7+58] = (byte)33;
      request[7+59] = (byte)48;
      request[7+60] = (byte)242;
      request[7+61] = (byte)16;
      request[7+62] = (byte)72;
      request[7+63] = (byte)41;
      request[7+64] = (byte)0;
      request[7+65] = (byte)48;
      request[7+66] = (byte)49;
      request[7+67] = (byte)55;
      request[7+68] = (byte)255;
      request[7+69] = (byte)51;
      request[7+70] = (byte)63;
      request[7+71] = (byte)74;
      request[7+72] = (byte)33;
      request[7+73] = (byte)76;
      request[7+74] = (byte)63;
      request[7+75] = (byte)75;
      request[7+76] = (byte)33;
      request[7+77] = (byte)239;
      request[7+78] = (byte)79;
      request[7+79] = (byte)63;
      request[7+80] = (byte)76;
      request[7+81] = (byte)33;
      request[7+82] = (byte)37;
      request[7+83] = (byte)0;
      request[7+84] = (byte)77;
      request[7+85] = (byte)33;
      request[7+86] = (byte)55;
      request[7+87] = (byte)255;
      request[7+88] = (byte)48;
      request[7+89] = (byte)48;
      request[7+90] = (byte)52;
      request[7+91] = (byte)49;
      request[7+92] = (byte)53;
      request[7+93] = (byte)48;
      request[7+94] = (byte)53;
      request[7+95] = (byte)52;
      request[7+96] = (byte)127;
      request[7+97] = (byte)55;
      request[7+98] = (byte)53;
      request[7+99] = (byte)54;
      request[7+100] = (byte)53;
      request[7+101] = (byte)51;
      request[7+102] = (byte)53;
      request[7+103] = (byte)49;
      request[7+104] = (byte)5;
      request[7+105] = (byte)1;
      request[7+106] = (byte)127;
      request[7+107] = (byte)78;
      request[7+108] = (byte)33;
      request[7+109] = (byte)48;
      request[7+110] = (byte)50;
      request[7+111] = (byte)63;
      request[7+112] = (byte)80;
      request[7+113] = (byte)33;
      request[7+114] = (byte)228;
      request[7+115] = (byte)23;
      request[7+116] = (byte)179;
      request[7+117] = (byte)63;
      request[7+118] = (byte)85;
      request[7+119] = (byte)47;
      request[7+120] = (byte)1;
      request[7+121] = (byte)112;
      request[7+122] = (byte)14;
      request[7+123] = (byte)63;
      request[7+124] = (byte)88;
      request[7+125] = (byte)96;
      request[7+126] = (byte)6;
      request[7+127] = (byte)63;
      request[7+128] = (byte)39;
      request[7+129] = (byte)89;
      request[7+130] = (byte)49;
      request[7+131] = (byte)49;
      request[7+132] = (byte)96;
      request[7+133] = (byte)5;
      request[7+134] = (byte)141;
      request[7+135] = (byte)0;
      request[7+136] = (byte)50;
      request[7+137] = (byte)41;
      request[7+138] = (byte)0;
      request[7+139] = (byte)141;
      request[7+140] = (byte)0;
      request[7+141] = (byte)171;
      request[7+142] = (byte)51;
      request[7+143] = (byte)33;
      request[7+144] = (byte)159;
      request[7+145] = (byte)0;
      request[7+146] = (byte)50;
      request[7+147] = (byte)144;
      request[7+148] = (byte)8;
      request[7+149] = (byte)50;
      request[7+150] = (byte)156;
      request[7+151] = (byte)3;
      request[7+152] = (byte)50;
      request[7+153] = (byte)170;
      request[7+154] = (byte)163;
      request[7+155] = (byte)2;
      request[7+156] = (byte)51;
      request[7+157] = (byte)144;
      request[7+158] = (byte)8;
      request[7+159] = (byte)51;
      request[7+160] = (byte)156;
      request[7+161] = (byte)3;
      request[7+162] = (byte)51;
      request[7+163] = (byte)163;
      request[7+164] = (byte)2;
      request[7+165] = (byte)52;
      request[7+166] = (byte)170;
      request[7+167] = (byte)144;
      request[7+168] = (byte)8;
      request[7+169] = (byte)52;
      request[7+170] = (byte)156;
      request[7+171] = (byte)3;
      request[7+172] = (byte)52;
      request[7+173] = (byte)163;
      request[7+174] = (byte)2;
      request[7+175] = (byte)53;
      request[7+176] = (byte)144;
      request[7+177] = (byte)8;
      request[7+178] = (byte)53;
      request[7+179] = (byte)122;
      request[7+180] = (byte)156;
      request[7+181] = (byte)3;
      request[7+182] = (byte)53;
      request[7+183] = (byte)163;
      request[7+184] = (byte)1;
      request[7+185] = (byte)69;
      request[7+186] = (byte)84;
      request[7+187] = (byte)88;
      request[7+188] = (byte)63;

      request[0]=0x00;
      request[1]=0x00;
      request[2]=0x60;
      request[3]=0x00;
      request[4]= 0x22;
      request[5]= 0x00 ;
      request[6]=0x00 ;
      request[1] = (byte)(request.Length-2);
      return request;
      lock (this)
      {
        byte[] compressed = new byte[] { 0x00, 0x00, 0x60, 0x00, 0x22, 0x00, 0x00 };
        byte[] header = new byte[] { 0x00, 0x00, 0x60, 0x00, 0x22, 0x00, 0x00 };
        byte[] headerLogon = new byte[] { 0x00, 0x00, 0x60, 0x00, 0x22, 0x00, 0x00, 0x7E, 0x01, 0x00 };
        byte[] headerLogoff = new byte[] { 0x00, 0x00, 0x60, 0x00, 0x22, 0x00, 0x00, 0x7E, 0x02, 0x00 };

        byte[] data = null;
        
        
        //if (counter > 999)
        //  counter = 0;

        //byte[] data = null;
        //if (counter % 2 == 0)
        //  data = headerLogon;
        //else
        //  data = headerLogoff;

        //data[1] = (byte)(data.Length - 4);
        return data;
        //return Encoding.ASCII.GetBytes(Encoding.ASCII.GetString(header) + request);
      }
    }

    public byte[] GenerateDiagnosticMessage()
    {
      return new byte[0];
    }

    public byte[] GenerateEchoMessage()
    {
      return new byte[0];
    }

    public byte[] GenerateWrongMessage()
    {
      return new byte[0];
    }
  }
}
