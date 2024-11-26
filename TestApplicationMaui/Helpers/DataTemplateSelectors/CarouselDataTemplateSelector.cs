using TestApplicationMaui.Models;

namespace TestApplicationMaui.Helpers.DataTemplateSelectors
{
    class CarouselDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TemplateA { get; set; }
        public DataTemplate TemplateB { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            // Choose the template based on the item's properties
            if (item is Person model)
            {
                return model.Age > 19 ? TemplateA : TemplateB;
            }

            return TemplateA; // Default template
        }
    }
}

