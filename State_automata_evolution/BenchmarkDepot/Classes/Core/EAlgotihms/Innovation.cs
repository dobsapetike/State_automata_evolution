using System;

namespace BenchmarkDepot.Classes.Core.EAlgotihms
{

    /// <summary>
    /// NEAT needs to maintain a list of all innovations that occurred during the generation/evolution,
    /// i.e. all unique structures, represented by the transducer states and the transition between them
    /// </summary>
    public class Innovation
    {

        #region Private fields

        /// <summary>
        /// Id of the first state
        /// </summary>
        private int _stateId1;
        public int FirstState
        {
            get { return _stateId1; }
        }

        /// <summary>
        /// Id of the second state
        /// </summary>
        private int _stateId2;
        public int SecondState
        {
            get { return _stateId2; }
        }


        /// <summary>
        /// The id number of the newly created state
        /// -1 if no no state was created
        /// </summary>
        private int _createdState;
        public int CreatedState
        {
            get { return _createdState; }
        }

        /// <summary>
        /// The id number of this innovation
        /// </summary>
        private int _innovationNumber;
        public int InnovationNumber
        {
            get { return _innovationNumber; }
        }

        #endregion

        #region Constructor

        public Innovation(int state1, int state2, int innovationNumber, int createdState = -1)
        {
            _stateId1 = state1;
            _stateId2 = state2;
            _innovationNumber = innovationNumber;
            _createdState = createdState;
        }

        #endregion

    }

}

