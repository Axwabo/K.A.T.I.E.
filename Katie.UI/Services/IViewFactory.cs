namespace Katie.UI.Services;

public interface IViewFactory
{

    Type ViewModelType { get; }

    Control Build(ViewModelBase viewModel);

}
