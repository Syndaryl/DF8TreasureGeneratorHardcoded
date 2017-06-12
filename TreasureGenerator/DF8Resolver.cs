
namespace Syndaryl.TreasureGenerator
{
    using DiceApp;
    using SoftCircuits;
    using Collections;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class DF8Resolver
    {
        readonly DF8Eval evaluator;
        readonly RollDice Dice;
        readonly Dictionary<string, WeightedList<df8CategoryItem>> categories;
        readonly TreeRoot<DF8Result> product = new TreeRoot<DF8Result>();

        public DF8Resolver(Dictionary<string, WeightedList<df8CategoryItem>> categories)
        {
            Dice = new RollDice();
            this.categories = categories;
            evaluator = new DF8Eval();
            evaluator.ProcessSymbol += ProcessSymbol;
            evaluator.ProcessFunction += ProcessFunction;
        }
        internal TreeRoot<DF8Result> ProduceFinalItem(df8CategoryItem item, TreeNode<DF8Result> parent = null)
        {
            if (item == null)
            {
                throw new Exception("I HAVE A NULL ITEM WTF");
                // yield break;
            }
            // First step - go down the tree of table results and rerolls and collect everything in order.
            if (parent == null)
            {
                var newParent = product.SetNode( new DF8Result(item));
            }
            ApplyChildrenRecursively(product.Node);
            ConvertToModifiers(product.Node);
            product.Traverse(ResolveItem);

            return product;
        }

        private void ConvertToModifiers(TreeNode<DF8Result> node)
        {
            if (node.Data.UnmodifiedItem == null )
                node.Data.UnmodifiedItem = (df8CategoryItem)node.Data.Item.Clone();

            var attributes = node.Data.UnmodifiedItem.GetAttributes().Where(x => !x.operation.Equals(""));
            if (attributes.Count() > 0)
            {
                node.Data.Modifiers.AddRange(attributes);
                //node.Data = null;
            }
            else
            {
                node.Data.UnmodifiedItem = (df8CategoryItem)node.Data.Item.Clone();
            }

            
        }

        void ResolveItem(DF8Result obj)
        {
            ResolveDiceRolls(obj);
            ResolveEquations(obj);
            ResolveQuantity(obj);
        }

        void ResolveQuantity(DF8Result input)
        {
            if (input.Item.GetAttribute("quantity") != null)
            {
                if (input.Item.GetSingleItemByEnum(ItemsChoiceType.Weight) != null)
                {
                    input.Item.Items[
                            input.Item.ItemsElementName.ToList().FindIndex(x => x == ItemsChoiceType.Weight)
                        ] 
                        = Convert.ToDouble(input.Item.GetSingleItemByEnum(ItemsChoiceType.Weight)) 
                          * Convert.ToDouble(input.Item.GetAttribute("quantity").value);
                }
            }
        }

        void ResolveEquations(DF8Result input)
        {
            evaluator.Current = input.Item;
            for (int i = 0; i < input.Item.ItemsElementName.Length; i++)
            {
                if (input.Item.ItemsElementName[i] != ItemsChoiceType.Attribute && input.Item.ItemsElementName[i] != ItemsChoiceType.Reroll)
                {
                    if (input.Item.Items[i] is string && ((string)input.Item.Items[i]).StartsWith("$", StringComparison.CurrentCulture))
                    {
                        input.Item.Items[i] = evaluator.Execute(((string)input.Item.Items[i]).Substring(1)).ToString();
                    }
                }
            }
        }

        private void ResolveDiceRolls(DF8Result input)
        {
            foreach (var attrib in input.Item.GetAttributes())
            {
                if (attrib.value.Contains('d')) // fast fail
                {
                    // if passes fast fail, use more expensive regex to confirm
                    var checkForDice = new System.Text.RegularExpressions.Regex(@"\d+d\d+");
                    if (checkForDice.IsMatch(attrib.value))
                    {
                        attrib.value = Dice.roll(attrib.value).ToString();
                    }
                }
            }
        }

        private void ApplyChildrenRecursively(TreeNode<DF8Result> node)
        {
            foreach (var categoryOfReroll in node.Data.Item.GetRerolls().Select(x => x.category))
            {
                var child = RandomFromCategory(categoryOfReroll);
                node.AddChildren(child);
            }
            foreach (var child in node.GetChildren())
            {
                ApplyChildrenRecursively(child);
            }
        }
        /*
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
        }*/

        private IEnumerable<DF8Result> RandomFromCategory(string category)
        {
            if (!categories.ContainsKey(category))
                return null;

            var roller = new WeightedListRoller<df8CategoryItem>(categories[category]);
            var randomItem = roller.RandomItem();
            var result = new List<DF8Result>();
            result.Add(new DF8Result(randomItem));
            return result;
        }
        
        /// <summary>
        /// Turn expression symbols into lookups of elements on the current item. Attribute must use a function (GetAttribute) due to the need to specify a name.
        /// </summary>
        /// <param name="sender">The Eval object being used to process the string.</param>
        /// <param name="e">The arguments being sent from Eval</param>
        protected void ProcessSymbol(object sender, SymbolEventArgs e)
        {
            if (Enum.GetNames(typeof(ItemsChoiceType)).Contains(e.Name) && evaluator.Current.ItemsElementName.Contains((ItemsChoiceType)Enum.Parse(typeof(ItemsChoiceType), e.Name)))
            {
                e.Result = Convert.ToDouble(evaluator.Current.GetSingleItemByEnum((ItemsChoiceType)Enum.Parse(typeof(ItemsChoiceType), e.Name)));
            }
            else if (evaluator.Current.ItemsElementName.Contains(ItemsChoiceType.Attribute) && e.Name.StartsWith("Attribute_", StringComparison.CurrentCulture))
            {
                var attribute = e.Name.Substring(10);
                e.Result = evaluator.Current.GetAttribute(attribute) == null ? 0 : evaluator.Execute(evaluator.Current.GetAttribute(attribute).value);
            }
            // Unknown symbol name
            else
            {
                e.Status = SymbolStatus.UndefinedSymbol;
            }
        }
        private void ProcessFunction(object sender, FunctionEventArgs e)
        {
            e.Status = FunctionStatus.UndefinedFunction;
        }
    }
}