// rolld6.cs
// arguments: <number of dice, integer>
using System;
using System.Text.RegularExpressions;

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

namespace DiceApp
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length == 0)
            	return -1;
            string arg = "";
			arg = String.Concat(args);
			var myRoller = new RollDice(arg);
			Console.WriteLine("{0}.", myRoller);
            return 0;
        }
    }

    public class RollDice
    {
        // static methods, accessable directly from Rolld6.whatever
        private static Random _r = new Random();
		
		public override string ToString(){
			string result = Convert.ToString(Rolls[0]);
			for (int x=1; x < Rolls.Length; x++){
				result = result + " + " + Convert.ToString(Rolls[x]);
			}
			
			if ( ( sign != null && ! sign.Equals("") ) || System.Math.Abs(offset) > Double.Epsilon) {
				result = result + " {";
				if ( sign != null && ! sign.Equals("") ){
					result = result + sign;
				}
				if ( offset > Double.Epsilon || offset < -1 * Double.Epsilon)
                {
					result = result + Convert.ToString(offset);
				}
				result = result + "}";
			}
			
			return result + " = " + Convert.ToString(sum);
		}
		
		private string sign;
		private double offset;
		private double sum;
		
		public double Sum{
			get
			{
				return sum;
			}
			set
			{
				sum = value;
			}
		}

        public RollDice(string line)
        {
//			Console.WriteLine("Calling RollDice constructor with args");
//			Console.WriteLine("Calling roll_array with {0}", line );
			roll(line);
		}
		
		public int[] roll_array(string line){
			//Console.WriteLine("Entering roll_array with {0}", line );
			int[] throws = new int[0];
			
			string pattern = @"^(\d+)?[dD](\d+|[%fF])";
			Match matcher = Regex.Match(line, pattern);
			
			if( ! matcher.Success ) {
				Console.WriteLine("{0} is not a valid dice string!", line );
				return throws;
			}
			
			int num = 1;
			string typeString = matcher.Groups[2].Value;
			
			int NumTest;
			bool isNum = int.TryParse(matcher.Groups[1].Value, out NumTest);
			
			if (isNum){
				num = NumTest;
			}
			
			//Console.WriteLine("Rolling {0} dice of typeString {1}",num, typeString);
			
			// rational bounds check.
			if ( num > 500 || num < 1 ) {
				Console.WriteLine("{0} is too many or too few dice, try with 1-500!",num);
				return throws;
			}
			
			if ( typeString.Equals("1") ) {
				throws = new int[1];
				throws[0] = 1;
			} else if (typeString.Equals("f", StringComparison.OrdinalIgnoreCase)){
				// Fudge Dice
				
				throws = new int[num];
				
				for (int x=0; x < num; x++) {
		            double n = _r.NextDouble();
		            throws[x] = (int)(n * 3) - 1;
				}
			} else if (! typeString.Equals("f", StringComparison.OrdinalIgnoreCase)){
				// Regular old dice
				if (typeString.Equals("%"))
					typeString = "100";
				int typeNum;
				NumTest = 0;
				isNum = int.TryParse(typeString, out NumTest);
				
				if (isNum){
					typeNum = NumTest;
				} else {
					Console.WriteLine("Out of cucumber error 1 dice typeString unmatched");
					return throws;
				}
				
				throws = new int[num];
				
				for (int x=0; x < num; x++) {
		            double n = _r.NextDouble();
		            throws[x] = (int)(n * typeNum) + 1;
				}
			} else {
				Console.WriteLine("Out of cucumber error 2 dice typeString unmatched");
				return throws;
			}
			Sum = 0;
			for (int x=0; x < throws.Length; x++){
				Sum += throws[x];
			}
			
			//Console.WriteLine("Rolls.Length {0}", throws.Length);
			Rolls = throws;
			return throws;
		}

		public double roll(string line){
			//^((?:\d+)?[dD](?:\d+|[%fF]))\s*(?:([\-+xX*/bBwW])(\d+.*))?
			int[] throws = new int[0];
			
			string pattern = @"^((?:\d+)?[dD](?:\d+|[%fF]))\s*(?:([\-+xX*/bBwW])(\d+.*))?";
			Match matcher = Regex.Match(line, pattern);
			
			if( ! matcher.Success ) {
				Console.WriteLine("{0} is not a valid dice string!", line );
				return 0;
			}
			
			string diceString = matcher.Groups[1].Value;
			sign = matcher.Groups[2].Value;
			string offsetString = matcher.Groups[3].Value;
			
			offset = 0;
			
			string typeString = matcher.Groups[2].Value;
			
			pattern = @"d";
			matcher = Regex.Match(offsetString, pattern);
			var OffsetRoll = new RollDice();
			if (matcher.Success) {
				OffsetRoll.roll_array(offsetString);
				offset = OffsetRoll.Sum;
				Console.WriteLine("Resolved {0} to {1}", offsetString, offset);
			} else {
				double NumTest = 0;
				bool isNum = double.TryParse(offsetString, out NumTest);
				
				if (isNum){
					offset = NumTest;
				}
			}
			
			throws = roll_array(diceString);
			
			if (throws.Length == 0)
				return 0;
			
			if (sign.Equals("b", StringComparison.OrdinalIgnoreCase)){
				// keep highest X dice
				sign = null;
			} else if  (sign.Equals("w", StringComparison.OrdinalIgnoreCase)){
				// keep lowest/worst X dice
				sign = null;
			}
			
			switch(sign)
			{
				case "+":
					Sum += offset;
					break;
				case "-":
					Sum -= offset;
					break;
				case "*":
					Sum *= offset;
					break;
				case @"/":
					Sum /= offset;
					break;
			}
			
			Rolls = throws;
			//if (OffsetRoll.Rolls.Length > 0){
			//	int rollcount = rolls.Length;
			//	Array.Resize<int>(ref rolls, rollcount + OffsetRoll.Rolls.Length);
			//	Array.Copy(OffsetRoll.Rolls, 0, rolls, rollcount, OffsetRoll.Rolls.Length);
			//}
			return Sum;
		}

        public RollDice()
        {
//			Console.WriteLine("Calling RollDice constructor with no args");
			Rolls = new int[0];
		}
		
		private int[] rolls;

        public int[] Rolls {
			get {
				return rolls;
			}
			set {
				rolls = value;
			}
		}
	}
}
