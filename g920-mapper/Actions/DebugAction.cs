using g920_mapper.Models;

namespace g920_mapper.Actions
{
	public class DebugAction
	{
		private List<byte> _keys;
		private WheelState? _wheelState;

		public DebugAction()
		{
			_keys = [];
		}

		public DebugAction SetWheelstate(WheelState? state)
		{
			_wheelState = state;

			return this;
		}

		public DebugAction SetKeys(List<byte> keys)
		{
			ArgumentNullException.ThrowIfNull(keys);

			_keys = keys;

			return this;
		}

		public void Execute()
		{
			ArgumentNullException.ThrowIfNull(_wheelState);

			Console.Clear();
			Console.WriteLine(@$"{DateTime.Now:G} 
Remember to disable debugging after evaluating!
--------------------------------------------------------
RAW_WHEEL_ROTATION:     {_wheelState.RAW_WHEEL_ROTATION} 
RAW_PEDAL_ACCELERATION:	{_wheelState.RAW_PEDAL_ACCELERATION}
RAW_PEDAL_BRAKE:        {_wheelState.RAW_PEDAL_BRAKE} 
RAW_PEDAL_CLUTCH:       {_wheelState.RAW_PEDAL_CLUTCH} 
--------------------------------------------------------
WHEEL_ROTATION_LEFT:    {(_wheelState.WHEEL_ROTATION_LEFT ? "true" : "")} 
WHEEL_ROTATION_RIGHT:   {(_wheelState.WHEEL_ROTATION_RIGHT ? "true" : "")} 
WHEEL_A:                {(_wheelState.WHEEL_A ? "true" : "")} 
WHEEL_B:                {(_wheelState.WHEEL_B ? "true" : "")} 
WHEEL_X:                {(_wheelState.WHEEL_X ? "true" : "")} 
WHEEL_Y:                {(_wheelState.WHEEL_Y ? "true" : "")} 
WHEEL_LB:               {(_wheelState.WHEEL_LB ? "true" : "")} 
WHEEL_RB:               {(_wheelState.WHEEL_RB ? "true" : "")} 
WHEEL_LSB:              {(_wheelState.WHEEL_LSB ? "true" : "")} 
WHEEL_RSB:              {(_wheelState.WHEEL_RSB ? "true" : "")} 
WHEEL_ACTION_RIGHT:     {(_wheelState.WHEEL_ACTION_RIGHT ? "true" : "")} 
WHEEL_ACTION_LEFT:      {(_wheelState.WHEEL_ACTION_LEFT ? "true" : "")} 
WHEEL_ARROW_UP:         {(_wheelState.WHEEL_ARROW_UP ? "true" : "")} 
WHEEL_ARROW_DOWN:       {(_wheelState.WHEEL_ARROW_DOWN ? "true" : "")} 
WHEEL_ARROW_LEFT:       {(_wheelState.WHEEL_ARROW_LEFT ? "true" : "")} 
WHEEL_ARROW_RIGHT:      {(_wheelState.WHEEL_ARROW_RIGHT ? "true" : "")} 
WHEEL_ACCELERATOR:      {(_wheelState.WHEEL_ACCELERATOR ? "true" : "")} 
WHEEL_BRAKE:            {(_wheelState.WHEEL_BRAKE ? "true" : "")} 
WHEEL_CLUTCH:           {(_wheelState.WHEEL_CLUTCH ? "true" : "")}
--------------------------------------------------------
Sending keys:           {string.Join(", ", _keys.Select(k => k.ToString()))}
");
		}
	}
}
