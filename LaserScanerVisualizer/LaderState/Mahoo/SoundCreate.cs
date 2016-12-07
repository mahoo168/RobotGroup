using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Runtime.InteropServices;
using System.Windows.Shapes;


namespace LaderState
{
    /// <summary>
    /// 音の生成
    /// </summary>
    class SoundCreate
    {
        public struct gakuhu
        {
            public List<System.Int32> sound_no;
            public GMProgram gakki;
            public int id;
            public int length;
            public int count;
            public int now_play_soundno;
            public int beat;
            public int sound_num;
            public gakuhu(List<System.Int32> list_sound_no, int a, int b, int c, int d, GMProgram t, int e, int f)
            {
                list_sound_no = new List<System.Int32>();
                sound_no = new List<System.Int32>(list_sound_no);
                id = a;
                now_play_soundno = f;
                length = b;
                count = c;
                beat = d;
                sound_num = e;
                gakki = t;
            }
        }
        public GMProgram Instrument;
        public IntPtr midi_1;
        public IntPtr midi_2;
        public byte basecode1 = 0;//アルペジエーター用
        public byte basecode2 = 0;//和音生成用1人目
        public byte basecode3 = 0;//和音生成用2人目
        public System.Timers.Timer timer_1;
        public int Sound_seed_num = 4;
        public int seed = Environment.TickCount;
        public int x1 = 5;
        public int y1 = 4;
        int count = 0;
        int[] C_code = { 0, 2, 4, 5, 7, 9, 11 };
        List<gakuhu> list_gakuhu;
        public System.Windows.Point Point_Sound_generate;
        public CIPC_CS.CLIENT.CLIENT cipc_reciever_client_sound_Point;
        public bool bool_start_sound = false;
        public bool bool_one = false;
        public int gakuhu_id_count = 0;
        int Remove_Count_forsoundseed = 0;
        public double pitch = 200;
        public SoundCreate()
        {
            midiOutOpen(out midi_1, -1, null, 0, CALLBACK.NULL);
            midiOutOpen(out midi_2, -1, null, 0, CALLBACK.NULL);
            Instrument = GMProgram.AcousticGrandPiano;
            Point_Sound_generate = new System.Windows.Point(0, 0);
            timer_1 = new System.Timers.Timer(1000);
            timer_1.Elapsed += new System.Timers.ElapsedEventHandler(minimam_music);
            timer_1.AutoReset = true;
            timer_1.Interval = pitch;
            list_gakuhu = new List<gakuhu>();
            midiOutShortMsg(midi_1, 0x9, 0, 0x45, 0).ToString();
            midiOutShortMsg(midi_2, 0x9, 0, 0x45, 0).ToString();
            for (int i = 0; i < 1; i++)
            {
                int rand = makerandom(0, 3);

                makegakuhu(list_gakuhu, 0);
            }

        }
        public void make_gakuhu_test()
        {
            int rand = makerandom(0, 3);
            makegakuhu(list_gakuhu, 2);
        }
        public void Sound_seed_evaluation(double speed)
        {
            int beat = 0;
            int rand = makerandom(0, 3);
            Sound_seed_num = (int)(map(speed, 0, 3, 0, 13));
            Console.WriteLine("num:" + Sound_seed_num);

            if (0 <= speed && speed < 0.5)
            {
                beat = 0;

                while (true)
                {
                    if (list_gakuhu.Count > Sound_seed_num)
                    {
                        list_gakuhu.RemoveAt(0);
                    }
                    else if (list_gakuhu.Count < Sound_seed_num)
                    {
                        makegakuhu(list_gakuhu, 0);
                    }
                    else if (list_gakuhu.Count == Sound_seed_num)
                    {
                        goto ExitLoop;
                    }
                }
                ExitLoop:;

            }
            else if (0.5 <= speed && speed < 0.6)
            {
                beat = 1;

                while (true)
                {
                    if (list_gakuhu.Count > Sound_seed_num)
                    {
                        list_gakuhu.RemoveAt(0);
                    }
                    else if (list_gakuhu.Count < Sound_seed_num)
                    {
                        makegakuhu(list_gakuhu, 1);
                    }
                    else if (list_gakuhu.Count == Sound_seed_num)
                    {
                        goto ExitLoop;
                    }
                }
                ExitLoop:;

            }
            else if (0.7 <= speed)
            {
                beat = 2;
                while (true)
                {
                    if (list_gakuhu.Count > Sound_seed_num)
                    {
                        list_gakuhu.RemoveAt(0);
                    }
                    else if (list_gakuhu.Count < Sound_seed_num)
                    {
                        makegakuhu(list_gakuhu, 2);
                    }
                    else if (list_gakuhu.Count == Sound_seed_num)
                    {
                        goto ExitLoop;
                    }
                }
                ExitLoop:;

            }
            if (list_gakuhu.Count >= 2)
            {
                list_gakuhu.RemoveAt(0);

                makegakuhu(list_gakuhu, rand);
            }
        }
        public int makeaddsound(int basesound_number)
        {
            int rand;
            int addsound_number = 0;
            switch (basesound_number)
            {
                case 0:
                    rand = makerandom(0, 5);
                    if (rand == 0) addsound_number = 4;
                    if (rand == 1) addsound_number = 5;
                    if (rand == 2) addsound_number = 7;
                    if (rand == 3) addsound_number = 9;
                    if (rand == 4) addsound_number = 12;
                    break;
                case 2:
                    rand = makerandom(0, 3);
                    if (rand == 0) addsound_number = 5;
                    if (rand == 1) addsound_number = 9;
                    if (rand == 2) addsound_number = 12;
                    break;
                case 4:
                    rand = makerandom(0, 3);
                    if (rand == 0) addsound_number = 3;
                    if (rand == 1) addsound_number = 8;
                    if (rand == 2) addsound_number = 12;
                    break;
                case 5:
                    rand = makerandom(0, 3);
                    if (rand == 0) addsound_number = 4;
                    if (rand == 1) addsound_number = 7;
                    if (rand == 2) addsound_number = 12;
                    break;
                case 7:
                    rand = makerandom(0, 3);
                    if (rand == 0) addsound_number = 4;
                    if (rand == 1) addsound_number = 7;
                    if (rand == 2) addsound_number = 12;
                    break;
                case 9:
                    rand = makerandom(0, 3);
                    if (rand == 0) addsound_number = 3;
                    if (rand == 1) addsound_number = 8;
                    if (rand == 2) addsound_number = 12;
                    break;
                case 11:
                    rand = makerandom(0, 3);
                    if (rand == 0) addsound_number = 3;
                    if (rand == 1) addsound_number = 8;
                    if (rand == 2) addsound_number = 12;
                    break;
            }
            return addsound_number;
        }
        public void start_soundmedia()
        {
            timer_1.Enabled = true;

        }
        public void change_pitch(double a)
        {
            this.timer_1.Interval = a;
        }
        public void makegakuhu(List<gakuhu> list_g, int beat)
        {
            //beat = 0~3 0->4beat 1->8beat 2->16beat
            gakuhu temp_gakuhu = new gakuhu();
            temp_gakuhu.id = 10000;
            temp_gakuhu.now_play_soundno = 0;
            temp_gakuhu.sound_no = new List<int>();
            temp_gakuhu.id = gakuhu_id_count;
            gakuhu_id_count++;
            switch (beat)
            {
                case 0:
                    temp_gakuhu.beat = 4;
                    break;
                case 1:
                    temp_gakuhu.beat = 8;
                    break;
                case 2:
                    temp_gakuhu.beat = 16;
                    break;
            }//beat決定
             //  Console.WriteLine("beat:" + temp_gakuhu.beat);
            switch (temp_gakuhu.beat)
            {
                case 4:
                    int num_rand_4 = makerandom(0, 2);
                    switch (num_rand_4)
                    {
                        case 0:
                            temp_gakuhu.sound_num = 3;
                            break;
                        case 1:
                            temp_gakuhu.sound_num = 4;
                            break;
                    }
                    break;
                //beatが4だった場合
                case 8:
                    int num_rand_8 = makerandom(0, 4);
                    switch (num_rand_8)
                    {
                        case 0:
                            temp_gakuhu.sound_num = 5;
                            break;
                        case 1:
                            temp_gakuhu.sound_num = 6;
                            break;
                        case 2:
                            temp_gakuhu.sound_num = 7;
                            break;
                        case 3:
                            temp_gakuhu.sound_num = 8;
                            break;
                    }
                    break;

                case 16:
                    int num_rand_16 = makerandom(0, 8);
                    switch (num_rand_16)
                    {
                        case 0:
                            temp_gakuhu.sound_num = 6;
                            break;
                        case 1:
                            temp_gakuhu.sound_num = 7;
                            break;
                        case 2:
                            temp_gakuhu.sound_num = 8;
                            break;
                        case 3:
                            temp_gakuhu.sound_num = 9;
                            break;
                        case 4:
                            temp_gakuhu.sound_num = 10;
                            break;
                        case 5:
                            temp_gakuhu.sound_num = 11;
                            break;
                        case 6:
                            temp_gakuhu.sound_num = 12;
                            break;
                        case 7:
                            temp_gakuhu.sound_num = 13;
                            break;
                    }
                    break;
            }
            // int gakki_rand = makerandom(0,2);
            int gakki_rand = 1;
            switch (gakki_rand)
            {
                case 0:
                    temp_gakuhu.gakki = GMProgram.Timpani;
                    break;
                case 1:
                    temp_gakuhu.gakki = GMProgram.AcousticGrandPiano;
                    break;


            }
            int rand_takasa = makerandom(0, 5);
            int musicheight = 0;
            switch (rand_takasa)
            {
                case 0:
                    musicheight = 3;
                    break;
                case 1:
                    musicheight = 6;
                    break;
                case 2:
                    musicheight = 4;
                    break;
                case 3:
                    musicheight = 5;
                    break;
                case 4:
                    musicheight = 2;
                    break;
            }
            //Console.WriteLine("musicheight:" + musicheight);
            int soundnum = 16 / temp_gakuhu.beat * temp_gakuhu.sound_num * 2;
            int beforesound = C_code[makerandom(0, 7)];
            int rand_1_or_two;

            switch (temp_gakuhu.beat)
            {
                case 4:
                    //4beat
                    for (int i = 0; i < soundnum; i++)
                    {
                        rand_1_or_two = makerandom(0, 2);

                        if (i % 4 == 0)
                        {
                            //音が入る部分
                            if (rand_1_or_two == 0)
                            {//単音
                                int soundno = makenextsound(beforesound % 12);
                                if (soundno != 400)
                                {
                                    temp_gakuhu.sound_no.Add((12 * musicheight) + soundno);
                                    beforesound = soundno;
                                }
                            }
                            else//複音
                            {
                                int soundno = makenextsound(beforesound % 12);
                                if (soundno != 400)
                                {
                                    temp_gakuhu.sound_no.Add((12 * musicheight) + soundno);
                                    int addsoundno = makeaddsound(soundno % 12);
                                    temp_gakuhu.sound_no.Add((12 * musicheight) + soundno + addsoundno);
                                    beforesound = soundno;
                                }
                            }
                            temp_gakuhu.sound_no.Add(0);
                        }
                        else
                        {
                            temp_gakuhu.sound_no.Add(0);
                        }
                    }
                    break;
                case 8:
                    //8beat
                    for (int i = 0; i < soundnum; i++)
                    {
                        rand_1_or_two = makerandom(0, 2);
                        if (i % 2 == 0)
                        {
                            //音が入る部分
                            if (rand_1_or_two == 0)
                            {//単音
                                int soundno = makenextsound(beforesound % 12);
                                if (soundno != 400)
                                {
                                    temp_gakuhu.sound_no.Add((12 * musicheight) + soundno);
                                    beforesound = soundno;
                                }
                            }
                            else//複音
                            {
                                int soundno = makenextsound(beforesound % 12);
                                if (soundno != 400)
                                {
                                    temp_gakuhu.sound_no.Add((12 * musicheight) + soundno);
                                    int addsoundno = makeaddsound(soundno % 12);
                                    temp_gakuhu.sound_no.Add((12 * musicheight) + soundno + addsoundno);
                                    beforesound = soundno;
                                }
                            }
                            temp_gakuhu.sound_no.Add(0);
                        }
                        else
                        {
                            temp_gakuhu.sound_no.Add(0);
                        }
                    }
                    break;
                case 16:
                    //16beat
                    for (int i = 0; i < soundnum; i++)
                    {
                        rand_1_or_two = makerandom(0, 2);

                        //音が入る部分
                        if (rand_1_or_two == 0)
                        {//単音
                            int soundno = makenextsound(beforesound % 12);
                            if (soundno != 400)
                            {
                                temp_gakuhu.sound_no.Add((12 * musicheight) + soundno);
                                beforesound = soundno;
                            }
                        }
                        else//複音
                        {
                            int soundno = makenextsound(beforesound % 12);
                            if (soundno != 400)
                            {
                                temp_gakuhu.sound_no.Add((12 * musicheight) + soundno);
                                int addsoundno = makeaddsound(soundno % 12);
                                temp_gakuhu.sound_no.Add((12 * musicheight) + soundno + addsoundno);
                                beforesound = soundno;
                            }
                        }
                        temp_gakuhu.sound_no.Add(0);
                    }
                    break;
            }

            list_g.Add(temp_gakuhu);
        }
        public int makerandom(int minvalue, int maxvalue)
        {
            byte[] bs = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng =
                new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bs);
            int i = System.BitConverter.ToInt32(bs, 0);
            double a = map((double)i, Int32.MinValue, Int32.MaxValue, minvalue, maxvalue);
            return (int)a;
        }
        public int makenextsound(int beforesound_number)
        {
            int rand;
            int soundnumber = 0;
            switch (beforesound_number)
            {
                case 0:
                    rand = makerandom(0, 7);
                    if (rand == 0) soundnumber = 0;
                    if (rand == 1) soundnumber = 4;
                    if (rand == 2) soundnumber = 5;
                    if (rand == 3) soundnumber = 7;
                    if (rand == 4) soundnumber = 9;
                    if (rand == 5) soundnumber = 12;
                    if (rand == 6) soundnumber = 400;//休み
                    break;
                case 2:
                    rand = makerandom(0, 5);
                    if (rand == 0) soundnumber = 2;
                    if (rand == 1) soundnumber = 7;
                    if (rand == 2) soundnumber = 11;
                    if (rand == 3) soundnumber = 14;
                    if (rand == 4) soundnumber = 400;//休み
                    break;
                case 4:
                    rand = makerandom(0, 5);
                    if (rand == 0) soundnumber = 4;
                    if (rand == 1) soundnumber = 7;
                    if (rand == 2) soundnumber = 12;
                    if (rand == 3) soundnumber = 16;
                    if (rand == 4) soundnumber = 400;//休み

                    break;
                case 5:
                    rand = makerandom(0, 6);
                    if (rand == 0) soundnumber = 0;
                    if (rand == 1) soundnumber = 5;
                    if (rand == 2) soundnumber = 9;
                    if (rand == 3) soundnumber = 12;
                    if (rand == 4) soundnumber = 17;
                    if (rand == 5) soundnumber = 400;//休み

                    break;
                case 7:
                    rand = makerandom(0, 6);
                    if (rand == 0) soundnumber = 2;
                    if (rand == 1) soundnumber = 7;
                    if (rand == 2) soundnumber = 11;
                    if (rand == 3) soundnumber = 14;
                    if (rand == 4) soundnumber = 19;
                    if (rand == 5) soundnumber = 400;//休み

                    break;
                case 9:
                    rand = makerandom(0, 6);
                    if (rand == 0) soundnumber = 21;
                    if (rand == 1) soundnumber = 5;
                    if (rand == 2) soundnumber = 9;
                    if (rand == 3) soundnumber = 12;
                    if (rand == 4) soundnumber = 17;
                    if (rand == 5) soundnumber = 400;//休み

                    break;
                case 11:
                    rand = makerandom(0, 6);
                    if (rand == 0) soundnumber = 2;
                    if (rand == 1) soundnumber = 7;
                    if (rand == 2) soundnumber = 11;
                    if (rand == 3) soundnumber = 14;
                    if (rand == 4) soundnumber = 19;
                    if (rand == 5) soundnumber = 400;//休み

                    break;
            }
            return soundnumber;
        }
        public void music_2(object sender, EventArgs e)
        {
            switch (count)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                case 7:
                    break;
                case 8:
                    break;
                case 9:
                    break;
                case 10:
                    break;
                case 11:
                    break;
                case 12:
                    break;
            }

        }
        public void minimam_music(object sender, EventArgs e)
        {
            try
            {
                // timer_1.Interval = pitch;
                for (int i = 0; i < list_gakuhu.Count; i++)
                {
                    while (true)
                    {
                        if (list_gakuhu[i].sound_no[list_gakuhu[i].now_play_soundno] != 0)
                        {
                            try
                            {
                                midiOutShortMsg(midi_1, 0xc, 10, list_gakuhu[i].gakki, 40);
                                midiOutShortMsg(midi_1, 0x9, 10, (byte)(list_gakuhu[i].sound_no[list_gakuhu[i].now_play_soundno]), 120).ToString();
                                //  Console.WriteLine("soundno:" + (byte)(list_gakuhu[i].sound_no[list_gakuhu[i].now_play_soundno]));
                                gakuhu gakuhu = list_gakuhu[i];
                                gakuhu.now_play_soundno++;
                                if (gakuhu.now_play_soundno >= list_gakuhu[i].sound_no.Count)
                                {
                                    gakuhu.now_play_soundno = 0;
                                }
                                list_gakuhu[i] = gakuhu;
                            }
                            catch
                            {

                            }

                        }
                        else
                        {
                            gakuhu gakuhu = list_gakuhu[i];
                            gakuhu.now_play_soundno++;
                            //    Console.WriteLine("soundno:" + (byte)(list_gakuhu[i].sound_no[list_gakuhu[i].now_play_soundno]));

                            if (gakuhu.now_play_soundno >= list_gakuhu[i].sound_no.Count)
                            {
                                gakuhu.now_play_soundno = 0;
                            }
                            list_gakuhu[i] = gakuhu;
                            goto Exitloop;
                        }

                    }
                    Exitloop:;
                }
            }
            catch { }
        }
        public void music_1(object sender, EventArgs e)
        {
            switch (count)
            {
                case 0:
                    midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                    midiOutShortMsg(midi_1, 0x9, 10, 81, 120).ToString();
                    midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                    midiOutShortMsg(midi_1, 0x9, 10, 69, 120).ToString();
                    //2
                    midiOutShortMsg(midi_1, 0xc, 10, GMProgram.SteelDrums, 40);
                    midiOutShortMsg(midi_1, 0x9, 10, 53, 120).ToString();
                    //3
                    count++;
                    break;
                case 1:
                    count++;
                    break;
                case 2:
                    midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                    midiOutShortMsg(midi_1, 0x9, 10, 81, 120).ToString();
                    midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                    midiOutShortMsg(midi_1, 0x9, 10, 69, 120).ToString();

                    count++;
                    break;
                case 3:
                    midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                    midiOutShortMsg(midi_1, 0x9, 10, 76, 120).ToString();
                    midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                    midiOutShortMsg(midi_1, 0x9, 10, 64, 120).ToString();


                    count++;
                    break;
                case 4:
                    midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                    midiOutShortMsg(midi_1, 0x9, 10, 76, 120).ToString();
                    midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                    midiOutShortMsg(midi_1, 0x9, 10, 64, 120).ToString();
                    count++;
                    break;
                case 5:
                    midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                    midiOutShortMsg(midi_1, 0x9, 10, 76, 120).ToString();
                    midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                    midiOutShortMsg(midi_1, 0x9, 10, 64, 120).ToString();


                    midiOutShortMsg(midi_1, 0xc, 10, GMProgram.SteelDrums, 40);
                    midiOutShortMsg(midi_1, 0x9, 10, 48, 120).ToString();
                    count++;
                    break;
                case 6:
                    count++;
                    break;
                case 7:
                    midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                    midiOutShortMsg(midi_1, 0x9, 10, 76, 120).ToString();
                    midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                    midiOutShortMsg(midi_1, 0x9, 10, 64, 120).ToString();
                    count = 0;
                    break;
            }
        }

        public void alpegiater2(int count)
        {
            //4,7,12,15
            switch (count)
            {
                case 0:
                    midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                    midiOutShortMsg(midi_1, 0x9, 10, basecode1, 120).ToString();

                    if (bool_one == true)
                    {
                        midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                        midiOutShortMsg(midi_1, 0x9, 10, basecode2, 120).ToString();
                        midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                        midiOutShortMsg(midi_1, 0x9, 10, (byte)(basecode2 + 4), 120).ToString();
                        midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                        midiOutShortMsg(midi_1, 0x9, 10, (byte)(basecode2 + 7), 120).ToString();
                        midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                        midiOutShortMsg(midi_1, 0x9, 10, (byte)(basecode2 + 12), 120).ToString();

                    }
                    break;
                case 1:
                    midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                    midiOutShortMsg(midi_1, 0x9, 10, basecode1, 0).ToString();

                    midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                    midiOutShortMsg(midi_1, 0x9, 10, (byte)(basecode1 + 4), 120).ToString();
                    break;
                case 2:
                    midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                    midiOutShortMsg(midi_1, 0x9, 10, (byte)(basecode1 + 4), 0).ToString();

                    midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                    midiOutShortMsg(midi_1, 0x9, 10, (byte)(basecode1 + 7), 120).ToString();
                    break;
                case 3:
                    midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                    midiOutShortMsg(midi_1, 0x9, 10, (byte)(basecode1 + 7), 0).ToString();
                    midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                    midiOutShortMsg(midi_1, 0x9, 10, (byte)(basecode1 + 12), 120).ToString();
                    if (bool_one == true)
                    {
                        midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                        midiOutShortMsg(midi_1, 0x9, 10, basecode2, 120).ToString();
                        midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                        midiOutShortMsg(midi_1, 0x9, 10, (byte)(basecode2 + 4), 120).ToString();
                        midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                        midiOutShortMsg(midi_1, 0x9, 10, (byte)(basecode2 + 7), 120).ToString();
                        midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                        midiOutShortMsg(midi_1, 0x9, 10, (byte)(basecode2 + 12), 120).ToString();
                    }
                    break;
                case 4:
                    midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                    midiOutShortMsg(midi_1, 0x9, 10, (byte)(basecode1 + 12), 0).ToString();

                    midiOutShortMsg(midi_1, 0xc, 10, Instrument, 40);
                    midiOutShortMsg(midi_1, 0x9, 10, (byte)(basecode1 + 16), 120).ToString();
                    break;

            }

        }




        public double map(double x, double in_min, double in_max, double out_min, double out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        public enum CALLBACK
        {
            EVENT = 0x50000,
            FUNCTION = 0x30000,
            NULL = 0x0,
            THREAD = 0x20000,
            WINDOW = 0x10000,
        }
        public static class MIDI
        {
            public const int MAPPAR = -1;
        }


        public static class MIDIERR
        {
            private const uint BASE = 64;
            public const uint NODEVICE = BASE + 4;
            public const uint BADOPENMODE = BASE + 6;
            public const uint NOTREADY = BASE + 3;
            public const uint STILLPLAYING = BASE + 1;

        }
        public static class MMSYSERR
        {
            private const uint BASE = 0;
            public const uint ALLOCATED = BASE + 4;
            public const uint BADDEVICEID = BASE + 2;
            public const uint INVALPARAM = BASE + 11;
            public const uint NOMEM = BASE + 7;
            public const uint INVALHANDLE = BASE + 5;
        }

        public delegate void MidiOutProc(
            IntPtr hmo,
            uint hwnd,
            int dwInstance,
            int dwParam1,
            int dwParam2
            );

        [DllImport("Winmm.dll")]
        public static extern uint midiOutGetNumDevs();


        [DllImport("Winmm.dll")]
        public static extern uint midiOutOpen(
            out IntPtr lphmo,
                int uDeviceID,
                MidiOutProc dwCallback,
                int dwCallbackInstance,
                CALLBACK dwFlags
            );

        [DllImport("Winmm.dll")]
        public static extern uint midiOutShortMsg(IntPtr hmo, int dwMsg);

        public static uint midiOutShortMsg(IntPtr hmo, byte status, byte channel, byte data1, byte data2)
        {
            return midiOutShortMsg(hmo, (status << 4) | channel | (data1 << 8) | (data2 << 16));
        }
        public static uint midiOutShortMsg(IntPtr hmo, byte status, byte channel, GMProgram data1, byte data2) { return midiOutShortMsg(hmo, (status << 4) | channel | ((byte)data1 << 8) | (data2 << 16)); }



        [DllImport("Winmm.dll")]
        public static extern uint midiOutReset(IntPtr hmo);


        [DllImport("Winmm.dll")]
        public static extern uint midiOutClose(IntPtr hmo);

        public enum GMProgram : byte
        { AcousticGrandPiano, BrightAcousticPiano, ElectricGrandPiano, HonkyTonkPiano, ElectricPiano1, ElectricPiano2, Harpsichord, Clavinet, Celesta, Glockenspiel, MusicBox, Vibraphone, Marimba, Xylophone, TubularBells, Dulcimer, DrawbarOrgan, PercussiveOrgan, RockOrgan, ChurchOrgan, ReedOrgan, Accordion, Harmonica, TangoAccordion, AcousticGuitarNylon, AcousticGuitarSteel, ElectricGuitarJazz, ElectricGuitarClean, ElectricGuitarMuted, OverdrivenGuitar, DistortionGuitar, GuitarHarmonics, AcousticBass, ElectricBassFinger, ElectricBassPick, FretlessBass, SlapBass1, SlapBass2, SynthBass1, SynthBass2, Violin, Viola, Cello, Contrabass, TremoloStrings, PizzicatoStrings, OrchestralHarp, Timpani, StringEnsemble1, StringEnsemble2, SynthStrings1, SynthStrings2, ChoirAahs, VoiceOohs, SynthVoice, OrchestraHit, Trumpet, Trombone, Tuba, MutedTrumpet, FrenchHorn, BrassSection, SynthBrass1, SynthBrass2, SopranoSax, AltoSax, TenorSax, BaritoneSax, Oboe, EnglishHorn, Bassoon, Clarinet, Piccolo, Flute, Recorder, PanFlute, BlownBottle, Shakuhachi, Whistle, Ocarina, Lead1Square, Lead2Sawtooth, Lead3Calliope, Lead4Chiff, Lead5Charang, Lead6Voice, Lead7Fifths, Lead8BassAndLead, Pad1NewAge, Pad2Warm, Pad3Polysynth, Pad4Choir, Pad5Bowed, Pad6Metallic, Pad7Halo, Pad8Sweep, Fx1Rain, Fx2Soundtrack, Fx3Crystal, Fx4Atmosphere, Fx5Brightness, Fx6Goblins, Fx7Echoes, Fx8SciFi, Sitar, Banjo, Shamisen, Koto, Kalimba, BagPipe, Fiddle, Shanai, TinkleBell, Agogo, SteelDrums, Woodblock, TaikoDrum, MelodicTom, SynthDrum, ReverseCymbal, GuitarFretNoise, BreathNoise, Seashore, BirdTweet, TelephoneRing, Helicopter, Applause, Gunshot }


    }
}
