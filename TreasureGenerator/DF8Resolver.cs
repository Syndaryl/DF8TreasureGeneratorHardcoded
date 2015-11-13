
namespace Syndaryl.TreasureGenerator
{
    using DiceApp;
    using SoftCircuits;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class DF8Resolver
    {
        readonly Eval evaluator;
        readonly RollDice Dice;
        public DF8Resolver()
        {
            Dice = new RollDice();
            evaluator = new SoftCircuits.Eval();
            evaluator.ProcessSymbol += ProcessSymbol;
            evaluator.ProcessFunction += ProcessFunction;
        }
        internal DF8Result ResolveItem(df8CategoryItem item)
        {
            if (item == null)
            {
                throw new Exception("I HAVE A NULL ITEM WTF");
                // yield break;
            }
            var rerollIndex = item.GetRerolls().ToList();
            var rerolledResults = ProcessRerolls(rerollIndex).ToList();

            throw new NotImplementedException();
        }


        private IEnumerable<df8CategoryItem> ProcessRerolls(IEnumerable<df8CategoryItemReroll> Rerolls)
        {
            foreach (df8CategoryItemReroll reroll in Rerolls)
            {
                string cat = reroll.category;
                foreach (var result in RandomFromCategory(cat).ToList())
                {
                    yield return (df8CategoryItem)result.Clone();
                }
            }
        }

        static df8CategoryItem ResolveQuantity(df8CategoryItem input)
        {
            var result = (df8CategoryItem)input.Clone();
            if (result.GetAttribute("quantity") != null)
            {
                if (result.GetSingleItemByEnum(ItemsChoiceType.Weight) != null)
                {
                    result.Items[result.ItemsElementName.ToList().FindIndex(x => x == ItemsChoiceType.Weight)] = Convert.ToDouble(result.GetSingleItemByEnum(ItemsChoiceType.Weight)) * Convert.ToDouble(result.GetAttribute("quantity").value);
                }
            }
            return result;
        }

        df8CategoryItem ResolveEquations(df8CategoryItem input)
        {
            var result = (df8CategoryItem)input.Clone();
            for (int i = 0; i < result.ItemsElementName.Length; i++)
            {
                if (result.ItemsElementName[i] != ItemsChoiceType.Attribute && result.ItemsElementName[i] != ItemsChoiceType.Reroll)
                {
                    if (result.Items[i] is string && ((string)result.Items[i]).StartsWith("$", StringComparison.CurrentCulture))
                    {
                        result.Items[i] = evaluator.Execute(((string)result.Items[i]).Substring(1)).ToString();
                    }
                }
            }
            return result;
        }

        df8CategoryItem ResolveDicerolls(df8CategoryItem input)
        {
            var result = (df8CategoryItem)input.Clone();
            foreach (var attrib in result.GetAttributes())
            {
                if (attrib.value.Contains('d'))
                {
                    var checkForDice = new System.Text.RegularExpressions.Regex(@"\d+d\d+");
                    if (checkForDice.IsMatch(attrib.value))
                    {
                        attrib.value = Dice.roll(attrib.value).ToString();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Turn expression symbols into lookups of elements on the current item. Attribute must use a function (GetAttribute) due to the need to specify a name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ProcessSymbol(object sender, SymbolEventArgs e)
        {
            if (currentItem == null)
            {
                // cant process any symbols if there's nothing to process them on!
                e.Status = SymbolStatus.UndefinedSymbol;
                return;
            }
            if (Enum.GetNames(typeof(ItemsChoiceType)).Contains(e.Name) && currentItem.ItemsElementName.Contains((ItemsChoiceType)Enum.Parse(typeof(ItemsChoiceType), e.Name)))
            {
                e.Result = Convert.ToDouble(currentItem.GetSingleItemByEnum((ItemsChoiceType)Enum.Parse(typeof(ItemsChoiceType), e.Name)));
            }
            else if (currentItem.ItemsElementName.Contains(ItemsChoiceType.Attribute) && e.Name.StartsWith("Attribute_", StringComparison.CurrentCulture))
            {
                var attribute = e.Name.Substring(10);
                e.Result = currentItem.GetAttribute(attribute) == null ? 0 : evaluator.Execute(currentItem.GetAttribute(attribute).value);
            }
            // Unknown symbol name
            else
            {
                e.Status = SymbolStatus.UndefinedSymbol;
            }
        }
        private void ProcessFunction(object sender, FunctionEventArgs e)
        {

            if (currentItem == null)
            {
                e.Status = FunctionStatus.UndefinedFunction;
                return;
            }
            e.Status = FunctionStatus.UndefinedFunction;
        }


    }
}