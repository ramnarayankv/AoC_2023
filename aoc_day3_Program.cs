using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;

// See https://aka.ms/new-console-template for more information

CustomLine[] myCustomLines;
/*
Dec  Char     
33  !        
35  #        
36  $        
37  %        
38  &        
39  '        
42  *        
43  +        
44  ,        
45  -        
47  /        
58  :        
59  ;        
60  <        
61  =        
62  >        
63  ?        
64  @
*/
/*
//Day 3 - part 1 
int[] myboundarr = new[] { 33, 35, 36, 37, 38, 39, 42, 43, 44, 45, 47, 58, 59, 60, 61, 62, 63, 64 };
 StringBuilder bound_str_pat = new StringBuilder("[");
*/

//Day 3 - part 2 
int[] myboundarr_part2 = new[] {  42 };

//Day 3 - part 2 
StringBuilder bound_str_pat_part2 = new StringBuilder("([");

//Day 3 - part 1 
/*
foreach (int bound_asc in myboundarr)
{ 
    bound_str_pat.Append(Regex.Escape(((char)bound_asc).ToString())).Append(@"|"); 
}
*/

//Day3 - Part 2
foreach (int bound_asc in myboundarr_part2)
{ 
    //Day 3 - part 2 
    bound_str_pat_part2.Append(Regex.Escape(((char)bound_asc).ToString())).Append(@"|"); 
}

//Day 3 - part 2 (Gear Ratio Calc)
 var str = bound_str_pat_part2.ToString().Trim('|') + "])";
//string bound_str_pat_val = str.Replace("$", @"\$").Replace("+", @"\+").Replace("?", @"\?").Replace("*", @"\*").Replace(",", @"\,").Replace("-",@"\-").Replace("/",@"\/");
string bound_str_pat_val = str.Replace("-",@"\-");
System.Diagnostics.Debug.WriteLine(bound_str_pat_val);
int ln_no = 0;
List<CustomLineMatch_part2> lstCustomLineMatch = new List<CustomLineMatch_part2>();
List<CustomLine> lstCustomLine = new List<CustomLine>();

foreach (string ln in File.ReadLines(@".\aoc_day3.txt"))
{
    var my_matchesAll = Regex.Matches(ln, @"(\d+)");
    var idx_Num = 0;
    lstCustomLine.Add(new CustomLine(ln, ln_no));
    var q1 = (from m in my_matchesAll
              select new CustomLineMatch_part2(
                  int.Parse(m.Groups[1].Value),
                  ln_no,
                  idx_Num++,
                  m.Index,
                  (m.Groups[1].Value).ToString().Length,
                  false,false,0,0)); 
    q1.ToList<CustomLineMatch_part2>().ForEach(x => lstCustomLineMatch.Add(x)); 
    ln_no++;
}

string i_ln_prev, i_ln_nxt = "";
char[] ln_prev_CharArr, ln_nxt_CharArr;

for (int j_ln = 0; j_ln < ln_no; j_ln++)
{
    var i_ln = lstCustomLine[j_ln].Line;
    //var i_idx_pos = lstCustomLineMatch[j_ln].Idx_Pos;
    //var i_len = lstCustomLineMatch[j_ln].Len;
    var i_ln_no = lstCustomLine[j_ln].LineNo;
    //var i_Num = lstCustomLineMatch[j_ln].Num;
    int i_ln_no_prev, i_ln_no_nxt = 0;
    var q1_line = from CustomLineMatch_part2 clm in lstCustomLineMatch
                  where clm.LineNo == i_ln_no
                  select clm;
    foreach (var c in q1_line.ToList<CustomLineMatch_part2>())
    {
        var i_str_pat_beg = string.Format(@"^({0})", c.Num);
        var i_str_pat_end = string.Format(@"({0})$", c.Num);
        var i_str_pat_left = string.Format(@"({0})\.", c.Num);
        var i_str_pat_right = string.Format(@"\.({0})", c.Num);
        var my_pat_right = "(" + bound_str_pat_val + i_str_pat_left + ")";
        var my_pat_left = "(" + i_str_pat_right + bound_str_pat_val + ")";
        var my_pat_beg = "(" + i_str_pat_beg + bound_str_pat_val + ")";
        var my_pat_end = "(" + bound_str_pat_val + i_str_pat_end + ")";
        var my_pat_left_right_beg_end = string.Format("{0}|{1}|{2}|{3}", my_pat_beg, my_pat_left, my_pat_right, my_pat_end);
        var my_matchesAll = Regex.Matches(i_ln, my_pat_left_right_beg_end);
        var lst_my_matchesAll = my_matchesAll.ToList();
        if (lst_my_matchesAll.Count > 0)
        {
            c.IsValid = true;
            c.SymbPos = (from Match m in lst_my_matchesAll
                         from Group g in m.Groups
                         where g.Value == "*"
                         select g.Index).FirstOrDefault();
            c.SymbLineNo = c.LineNo;
            Debug.WriteLine("Default match");
            Debug.WriteLine("Adding valid num: " + c.Num + " in line :" + c.LineNo + " ; Position: " + c.Idx_Pos);
        }
        else
        {
            bool nxtBound = false;
            bool isMid = false;
            if (i_ln_no < ln_no - 1)
            {
                i_ln_no_nxt = lstCustomLine[j_ln + 1].LineNo;
                i_ln_nxt = lstCustomLine[j_ln + 1].Line;
                ln_nxt_CharArr = (i_ln_nxt).ToCharArray();
                var k_low = c.Idx_Pos == 0 ? 0 : c.Idx_Pos - 1;
                var k_high = c.Idx_Pos + c.Len + 1 >= 140 ? 140 : c.Idx_Pos + c.Len + 1;
                for (int kNxt = k_low; kNxt < k_high; kNxt++)
                {
                    if (Regex.IsMatch(ln_nxt_CharArr[kNxt].ToString(), bound_str_pat_val))
                    {
                        c.SymbLineNo = i_ln_no_nxt;
                        c.SymbPos = kNxt;
                        nxtBound = true;
                        break;
                    }
                }
            }
            if ((i_ln_no > 0) && !nxtBound)
            {
                i_ln_no_prev = lstCustomLine[j_ln - 1].LineNo;
                i_ln_prev = lstCustomLine[j_ln - 1].Line;
                ln_prev_CharArr = (i_ln_prev).ToCharArray();
                var l_low = c.Idx_Pos == 0 ? 0 : c.Idx_Pos - 1;
                var l_high = c.Idx_Pos + c.Len + 1 >= 140 ? 140 : c.Idx_Pos + c.Len + 1;
                for (int lNxt = l_low; lNxt < l_high; lNxt++)
                {
                    if (Regex.IsMatch(ln_prev_CharArr[lNxt].ToString(), bound_str_pat_val))
                    {
                        c.SymbLineNo = i_ln_no_prev;
                        c.SymbPos = lNxt;
                        nxtBound = true;
                        break;
                    }
                }
            }
            if (nxtBound)
            {
                Debug.WriteLine("prev/next line match");
                c.IsValid = true;
                Debug.WriteLine("Adding valid num: " + c.Num.ToString() + " in line :" + c.LineNo.ToString() + " ; Position:  " + c.Idx_Pos);
            } 
        }
    }
}

var res_q = from CustomLineMatch_part2 clm_res in lstCustomLineMatch
            where clm_res.IsValid == true 
            select clm_res;
int sumGearRatio = 0;
var lstRes = res_q.ToList<CustomLineMatch_part2>();
var lst_uniq = (from CustomLineMatch_part2 u in lstRes
               select (u.SymbLineNo + "_" + u.SymbPos)).Distinct().ToList();
var lstGearRatio = new List<GearRatioItem>();
int i_gr = 0;
lst_uniq.ForEach(u =>
{
   var lstGearRatio_itm = (from CustomLineMatch_part2 gr1 in lstRes
                       where gr1.SymbLineNo + "_" + gr1.SymbPos == u
                       select gr1).ToList();
    if (lstGearRatio_itm.Count == 2)
    {
        int i_gr1 = lstGearRatio_itm.First().Num;
        int i_gr2 = lstGearRatio_itm.Last().Num;
        int i_prod = i_gr1 * i_gr2;
        GearRatioItem newGRItem = new GearRatioItem(i_gr++, i_gr1, i_gr2, i_prod);
        lstGearRatio.Add(newGRItem);
    }
});
lstGearRatio.ForEach(s => 
{ 
    sumGearRatio += s.GRatioProduct; 
    Debug.WriteLine("Gear Ration for GR1: " + s.GRatioVal1 + " ; GR2: " + s.GRatioVal2 + " ; Gear Ration product: " + s.GRatioProduct );
});
System.Diagnostics.Debug.WriteLine("Count of valid part num is: " +
    lstRes.Count.ToString()
    );
System.Diagnostics.Debug.WriteLine("Gear Ratio Sum is: " + sumGearRatio);
Console.ReadLine();

//
// End Day 3 - Part 2 (Gear Ratio Calc)

#region Day 3 - Part 1
/* 
 * //Day 3 - Part 1
var str = bound_str_pat.ToString().Trim('|') + "]";
//string bound_str_pat_val = str.Replace("$", @"\$").Replace("+", @"\+").Replace("?", @"\?").Replace("*", @"\*").Replace(",", @"\,").Replace("-",@"\-").Replace("/",@"\/");
string bound_str_pat_val = str.Replace("-",@"\-");
System.Diagnostics.Debug.WriteLine(bound_str_pat_val);
int ln_no = 0;
List<CustomLineMatch> lstCustomLineMatch = new List<CustomLineMatch>();
List<CustomLine> lstCustomLine = new List<CustomLine>();

foreach (string ln in File.ReadLines(@".\aoc_day3.txt"))
{
    var my_matchesAll = Regex.Matches(ln, @"(\d+)");
    var idx_Num = 0;
    lstCustomLine.Add(new CustomLine(ln, ln_no));
    var q1 = (from m in my_matchesAll
              select new CustomLineMatch(
                  int.Parse(m.Groups[1].Value),
                  ln_no,
                  idx_Num++,
                  m.Index,
                  (m.Groups[1].Value).ToString().Length,
                  false,false)); 
    q1.ToList<CustomLineMatch>().ForEach(x => lstCustomLineMatch.Add(x)); 
    ln_no++;
}

string i_ln_prev, i_ln_nxt = "";
char[] ln_prev_CharArr, ln_nxt_CharArr;

for (int j_ln = 0; j_ln < ln_no; j_ln++)
{
    var i_ln = lstCustomLine[j_ln].Line;
    //var i_idx_pos = lstCustomLineMatch[j_ln].Idx_Pos;
    //var i_len = lstCustomLineMatch[j_ln].Len;
    var i_ln_no = lstCustomLine[j_ln].LineNo;
    //var i_Num = lstCustomLineMatch[j_ln].Num;
    int i_ln_no_prev, i_ln_no_nxt = 0;
    var q1_line = from CustomLineMatch clm in lstCustomLineMatch
                  where clm.LineNo == i_ln_no
                  select clm;
    foreach (var c in q1_line.ToList<CustomLineMatch>())
    {
        var i_str_pat_beg = string.Format(@"^({0})", c.Num);
        var i_str_pat_end = string.Format(@"({0})$", c.Num);
        var i_str_pat_left = string.Format(@"({0})\.", c.Num);
        //var i_str_pat_left_alt = string.Format(@"({0})", c.Num);
        var i_str_pat_right = string.Format(@"\.({0})", c.Num);
        var my_pat_right = "(" + bound_str_pat_val + i_str_pat_left + ")";
        //var my_pat_left_alt = "(" + bound_str_pat_val + i_str_pat_left_alt + bound_str_pat_val + ")";
        var my_pat_left = "(" + i_str_pat_right + bound_str_pat_val + ")";
        var my_pat_beg  = "(" + i_str_pat_beg + bound_str_pat_val + ")";
        var my_pat_end  = "(" + bound_str_pat_val + i_str_pat_end + ")";
        var my_pat_left_right_beg_end = string.Format("{0}|{1}|{2}|{3}",my_pat_beg,my_pat_left, my_pat_right,my_pat_end);
        var my_matchesAll = Regex.Matches(i_ln, my_pat_left_right_beg_end);
        var lst_my_matchesAll = my_matchesAll.ToList();
        if(lst_my_matchesAll.Count > 0)
        {
            c.IsValid = true;
            c.IsMid = false;

            Debug.WriteLine("Default match");
            Debug.WriteLine("Adding valid num: " + c.Num + " in line :" + c.LineNo); 
        } 
        else 
        {
            bool nxtBound = false;
            bool isMid = false;
            if (i_ln_no < ln_no-1)
            {
                i_ln_no_nxt = lstCustomLine[j_ln + 1].LineNo;
                i_ln_nxt = lstCustomLine[j_ln + 1].Line;
                ln_nxt_CharArr = (i_ln_nxt).ToCharArray();
                var k_low = c.Idx_Pos == 0 ?0:c.Idx_Pos-1;
                var k_high = c.Idx_Pos + c.Len + 1 >= 140 ? 140 : c.Idx_Pos + c.Len + 1; 
                for(int kNxt = k_low; kNxt < k_high; kNxt++)
                {
                    if (Regex.IsMatch(ln_nxt_CharArr[kNxt].ToString(), bound_str_pat_val)) 
                    {
                        if (k_low !=0 && k_high !=140 && kNxt >= k_low+2 && kNxt < k_high-2 && c.Len >= 3) { isMid = true; }
                        nxtBound = true;
                        break;
                    }
                } 
            }
            if( (i_ln_no > 0) && !nxtBound)
            {
                i_ln_no_prev = lstCustomLine[j_ln - 1].LineNo;
                i_ln_prev = lstCustomLine[j_ln - 1].Line;
                ln_prev_CharArr = (i_ln_prev).ToCharArray();
                var l_low = c.Idx_Pos == 0 ? 0:c.Idx_Pos-1;
                var l_high = c.Idx_Pos + c.Len + 1 >= 140 ? 140 : c.Idx_Pos + c.Len + 1; 
                for(int lNxt = l_low; lNxt < l_high;  lNxt++)
                {
                    if (Regex.IsMatch(ln_prev_CharArr[lNxt].ToString(), bound_str_pat_val)) 
                    {

                        if (l_low !=0 && l_high != 140 && lNxt >= l_low+2 && lNxt < l_high-2 && c.Len>=3) { isMid = true; }
                        nxtBound = true;
                        break;
                    }
                } 
            }
            if (nxtBound)
            {
                Debug.WriteLine("prev/next line match");
                c.IsValid = true;
                c.IsMid = isMid;
                Debug.WriteLine("Adding valid num: " + c.Num + " in line :" + c.LineNo + ".Is it Middle? " + c.IsMid);
            } 

        }

    }
}

var res_q = from CustomLineMatch clm_res in lstCustomLineMatch
            where clm_res.IsValid == true 
            select clm_res;
int sum = 0;
int count_isValid = 0;
var lstRes = res_q.ToList<CustomLineMatch>();
lstRes.ForEach(y => sum += y.Num);
System.Diagnostics.Debug.WriteLine("Count of valid part num is: " + 
    lstRes.Count.ToString()
    );
System.Diagnostics.Debug.WriteLine("Sum is: " + sum);
Console.ReadLine();
// End Day 3 - Part 1
*/
#endregion Day 3 - Part 1


//Day 3 - Part 2
public class GearRatioItem 
{
    public int GRIdx { get; set; }
    public int GRatioVal1 { get; set; }
    public int GRatioVal2 { get; set; }
    public int GRatioProduct { get; set; }
    public GearRatioItem(int gr_idx, int gr_val1, int gr_val2, int gr_prod)
    {
        GRIdx = gr_idx;
        GRatioVal1 = gr_val1;
        GRatioVal2 = gr_val2;
        GRatioProduct = gr_prod;

    } 
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        { return false; }

        GearRatioItem otherGRItem = (GearRatioItem)obj;
        return (GRatioVal1 == otherGRItem.GRatioVal1 && GRatioVal2 == otherGRItem.GRatioVal2)
            || (GRatioVal1 == otherGRItem.GRatioVal2 && GRatioVal2 == otherGRItem.GRatioVal1); 
    }
}
public class CustomLine
{
    public CustomLine(string ln, int line_no)
    {
        Line = ln;
        LineNo = line_no;
    }
    public int LineNo { get; set; }
    public string Line { get; set; }

   
}

public class CustomLineMatch_part2 : CustomLineMatch
{
    public int SymbPos { get; set; }
    public int SymbLineNo { get; set; }
    public CustomLineMatch_part2(int num, int line_no, int idx_num, int idx_pos, int len, bool isVal,bool isMid,int symb_pos,int symb_ln_no) : base(num,line_no,idx_num,idx_pos,len,isVal,isMid) 
    {
        SymbPos = symb_pos;
        SymbLineNo = symb_ln_no;
    }
}
public class CustomLineMatch
{
    public CustomLineMatch(int num, int line_no, int idx_num, int idx_pos, int len, bool isVal,bool isMid)
    {
        LineNo = line_no;
        Num = num;
        Idx_Num = idx_num;
        Idx_Pos = idx_pos;
        Len = len;
        IsValid = isVal;
        IsMid = isMid;
    }
    public int LineNo { get; set; }
    public int Num { get; set; }
    public int Idx_Num { get; set; }
    public int Idx_Pos { get; set; }
    public int Len { get; set; }
    public bool IsValid { get; set; }
    public bool IsMid { get; set; }
}
