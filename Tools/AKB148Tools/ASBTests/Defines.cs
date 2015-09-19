using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASBTests
{
    class Members
    {
        static List<string> memberList = new List<string> { "Player", "Misaki Iwasa", "Tada Manaka", "Shizuka Oya", "Haruka Katayama", "Kuramochi Asuka", "Haruna Kojima", "Rino Sashihara", "Mariko Shinoda", "Takashiro Aki", "Takahashi Minami", "Nakagawa Haruka", "Chisato Nakata", "Nakaya Sayaka", "Atsuko Maeda", "Maeda Ami", "Natsumi Matsubara", "Sayaka Akimoto", "Itano Tomomi", "Mayumi Uchida", "Ayaka Umeda", "Yuko Oshima", "Ayaka Kikuchi", "Miku Tanabe", "Nakatsuka Tomomi", "Moeno Nito", "Nonaka Misato", "Fujie Reina", "Sakiko Matsui", "Minami Minegishi", "Sae Miyazawa", "Yokoyama Yui", "Yonezawa Rumi", "Haruka Ishida", "Tomomi Kasai", "Kashiwagi Yuki", "Rie Kitahara", "Kana Kobayashi", "Mika Komori", "Sato Amina", "Sumire Sato", "Natsuki Sato", "Mariya Suzuki", "Konno Rina", "Hirashima Natsumi", "Yuka Masuda", "Miyazaki Miho", "Mayu Watanabe", "Shihori Suzuki" };

        public static string getMemberName(int number)
        {
            if (number > memberList.Count)
                return number.ToString("x4");
            return "\"" + memberList[number]+ "\"";
        }
    }
}
