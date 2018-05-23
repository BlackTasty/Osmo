using System;

namespace Osmo.ViewModel
{
    class MasterViewModel
    {
        private static ViewModelBase instance;

        public static ViewModelBase GetInstance()
        {
            if (instance == null)
                throw new NullReferenceException("The static method SetInstance(ViewModelBase viewModel) has to be called first!");

            return instance;
        }

        public static void SetInstance(ViewModelBase viewModel)
        {
            if (viewModel == null)
                throw new NullReferenceException("You cannot set null as a master viewmodel!");

            if (instance != null)
                instance = viewModel;
        }
    }
}
