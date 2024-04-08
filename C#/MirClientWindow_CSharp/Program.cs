using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

using BOOL = System.Int32;
using size_t = System.IntPtr;

using MirConnection = System.IntPtr;
using MirDisplayConfig = System.IntPtr;
using MirOutput = System.IntPtr;
using MirOutputMode = System.IntPtr;
using MirWindowSpec = System.IntPtr;
using MirWindow = System.IntPtr;
using MirBufferStream = System.IntPtr;
using MirWaitHandle = System.IntPtr;
using MirEvent = System.IntPtr;

using MirWindowEventCallback = System.IntPtr;

namespace TestMirCSharp
{
	enum MirOutputConnectionState
	{
		mir_output_connection_state_disconnected = 0,
		mir_output_connection_state_connected,
		mir_output_connection_state_unknown
	}

	enum MirPixelFormat
	{
		mir_pixel_format_invalid = 0,
		mir_pixel_format_abgr_8888 = 1,
		mir_pixel_format_xbgr_8888 = 2,
		mir_pixel_format_argb_8888 = 3,
		mir_pixel_format_xrgb_8888 = 4,
		mir_pixel_format_bgr_888 = 5,
		mir_pixel_format_rgb_888 = 6,
		mir_pixel_format_rgb_565 = 7,
		mir_pixel_format_rgba_5551 = 8,
		mir_pixel_format_rgba_4444 = 9,
		mir_pixel_formats   /* Note: This is always max format + 1 */
	}

	enum MirBufferUsage
	{
		mir_buffer_usage_hardware = 1,
		mir_buffer_usage_software
	}

	enum MirEventType
	{
		mir_event_type_key,//MIR_DEPRECATED_ENUM(mir_event_type_key, "mir_event_type_input"),     // UNUSED since Mir 0.26
		mir_event_type_motion,//MIR_DEPRECATED_ENUM(mir_event_type_motion, "mir_event_type_input"),  // UNUSED since Mir 0.26
		mir_event_type_surface,//MIR_DEPRECATED_ENUM(mir_event_type_surface, "mir_event_type_window"),
		mir_event_type_window = mir_event_type_surface,
		mir_event_type_resize,
		mir_event_type_prompt_session_state_change,
		mir_event_type_orientation,
		mir_event_type_close_surface,//MIR_DEPRECATED_ENUM(mir_event_type_close_surface, "mir_event_type_close_window"),
		mir_event_type_close_window = mir_event_type_close_surface,
		/* Type for new style input event will be returned from mir_event_get_type
		when old style event type was mir_event_type_key or mir_event_type_motion */
		mir_event_type_input,
		mir_event_type_keymap,
		mir_event_type_input_configuration,//MIR_DEPRECATED_ENUM(mir_event_type_input_configuration, "mir_connection_set_input_config_change_callback and mir_event_type_input_device_state"),
		mir_event_type_surface_output,//MIR_DEPRECATED_ENUM(mir_event_type_surface_output, "mir_event_type_window_output"),
		mir_event_type_window_output = mir_event_type_surface_output,
		mir_event_type_input_device_state,
		mir_event_type_surface_placement,//MIR_DEPRECATED_ENUM(mir_event_type_surface_placement, "mir_event_type_window_placement"),
		mir_event_type_window_placement = mir_event_type_surface_placement,
	}

	[StructLayout(LayoutKind.Sequential)]
	unsafe struct MirGraphicsRegion
	{
		public int width;
		public int height;
		public int stride;
		public MirPixelFormat pixel_format;
		public byte* vaddr;

	}

	static unsafe class MainClass
	{
		const string clientLib = "libmirclient.so";

		[DllImport(clientLib, EntryPoint = "mir_connect_sync", ExactSpelling = true)]
		public static extern MirConnection mir_connect_sync(byte* server, byte* app_name);

		[DllImport(clientLib, EntryPoint = "mir_connection_release", ExactSpelling = true)]
		public static extern void mir_connection_release(MirConnection connection);

		[DllImport(clientLib, EntryPoint = "mir_connection_is_valid", ExactSpelling = true)]
		public static extern BOOL mir_connection_is_valid(MirConnection connection);

		[DllImport(clientLib, EntryPoint = "mir_connection_is_valid", ExactSpelling = true)]
		public static extern byte* mir_connection_get_error_message(MirConnection connection);

		[DllImport(clientLib, EntryPoint = "mir_connection_create_display_configuration", ExactSpelling = true)]
		public static extern MirDisplayConfig mir_connection_create_display_configuration(MirConnection connection);

		[DllImport(clientLib, EntryPoint = "mir_display_config_get_num_outputs", ExactSpelling = true)]
		public static extern int mir_display_config_get_num_outputs(MirDisplayConfig config);

		[DllImport(clientLib, EntryPoint = "mir_display_config_get_output", ExactSpelling = true)]
		public static extern MirOutput mir_display_config_get_output(MirDisplayConfig config, size_t index);

		[DllImport(clientLib, EntryPoint = "mir_output_get_connection_state", ExactSpelling = true)]
		public static extern MirOutputConnectionState mir_output_get_connection_state(MirOutput output);

		[DllImport(clientLib, EntryPoint = "mir_output_is_enabled", ExactSpelling = true)]
		public static extern BOOL mir_output_is_enabled(MirOutput output);

		[DllImport(clientLib, EntryPoint = "mir_output_get_num_pixel_formats", ExactSpelling = true)]
		public static extern int mir_output_get_num_pixel_formats(MirOutput output);

		[DllImport(clientLib, EntryPoint = "mir_output_get_pixel_format", ExactSpelling = true)]
		public static extern MirPixelFormat mir_output_get_pixel_format(MirOutput output, size_t index);

		[DllImport(clientLib, EntryPoint = "mir_output_get_current_mode", ExactSpelling = true)]
		public static extern MirOutputMode mir_output_get_current_mode(MirOutput output);

		[DllImport(clientLib, EntryPoint = "mir_display_config_release", ExactSpelling = true)]
		public static extern void mir_display_config_release(MirDisplayConfig config);

		[DllImport(clientLib, EntryPoint = "mir_output_mode_get_width", ExactSpelling = true)]
		public static extern int mir_output_mode_get_width(MirOutputMode mode);

		[DllImport(clientLib, EntryPoint = "mir_output_mode_get_height", ExactSpelling = true)]
		public static extern int mir_output_mode_get_height(MirOutputMode mode);

		[DllImport(clientLib, EntryPoint = "mir_create_normal_window_spec", ExactSpelling = true)]
		public static extern MirWindowSpec mir_create_normal_window_spec(MirConnection connection, int width, int height);

		[DllImport(clientLib, EntryPoint = "mir_window_spec_set_pixel_format", ExactSpelling = true)]
		public static extern void mir_window_spec_set_pixel_format(MirWindowSpec spec, MirPixelFormat format);

		[DllImport(clientLib, EntryPoint = "mir_window_spec_set_name", ExactSpelling = true)]
		public static extern void mir_window_spec_set_name(MirWindowSpec spec, byte* name);

		[DllImport(clientLib, EntryPoint = "mir_window_spec_set_buffer_usage", ExactSpelling = true)]
		public static extern void mir_window_spec_set_buffer_usage(MirWindowSpec spec, MirBufferUsage usage);

		[DllImport(clientLib, EntryPoint = "mir_create_window_sync", ExactSpelling = true)]
		public static extern MirWindow mir_create_window_sync(MirWindowSpec requested_specification);

		[DllImport(clientLib, EntryPoint = "mir_window_spec_release", ExactSpelling = true)]
		public static extern void mir_window_spec_release(MirWindowSpec spec);

		[DllImport(clientLib, EntryPoint = "mir_window_get_buffer_stream", ExactSpelling = true)]
		public static extern MirBufferStream mir_window_get_buffer_stream(MirWindow window);

		[DllImport(clientLib, EntryPoint = "mir_buffer_stream_set_swapinterval", ExactSpelling = true)]
		public static extern MirWaitHandle mir_buffer_stream_set_swapinterval(MirBufferStream stream, int interval);

		[DllImport(clientLib, EntryPoint = "mir_window_set_event_handler", ExactSpelling = true)]
		public static extern void mir_window_set_event_handler(MirWindow window, MirWindowEventCallback callback, void* context);

		[DllImport(clientLib, EntryPoint = "mir_window_release_sync", ExactSpelling = true)]
		public static extern void mir_window_release_sync(MirWindow window);

		[DllImport(clientLib, EntryPoint = "mir_buffer_stream_get_graphics_region", ExactSpelling = true)]
		public static extern BOOL mir_buffer_stream_get_graphics_region(MirBufferStream buffer_stream, MirGraphicsRegion* graphics_region);

		[DllImport(clientLib, EntryPoint = "mir_buffer_stream_swap_buffers_sync", ExactSpelling = true)]
		public static extern void mir_buffer_stream_swap_buffers_sync(MirBufferStream buffer_stream);

		[DllImport(clientLib, EntryPoint = "mir_event_get_type", ExactSpelling = true)]
		public static extern MirEventType mir_event_get_type(MirEvent e);

		static bool running = true;
		static int swap_interval;

		static int BYTES_PER_PIXEL(MirPixelFormat format)
		{
			switch (format)
			{
				case MirPixelFormat.mir_pixel_format_abgr_8888:
				case MirPixelFormat.mir_pixel_format_xbgr_8888:
				case MirPixelFormat.mir_pixel_format_argb_8888:
				case MirPixelFormat.mir_pixel_format_xrgb_8888:
					return 4;

				case MirPixelFormat.mir_pixel_format_bgr_888:
				case MirPixelFormat.mir_pixel_format_rgb_888:
					return 3;

				case MirPixelFormat.mir_pixel_format_rgb_565:
				case MirPixelFormat.mir_pixel_format_rgba_5551:
				case MirPixelFormat.mir_pixel_format_rgba_4444:
					return 2;
			}
			return 0;
		}

		static MirOutput find_active_output(MirDisplayConfig conf)
		{
			int num_outputs = mir_display_config_get_num_outputs(conf);
			for (int i = 0; i < num_outputs; i++)
			{
				MirOutput output = mir_display_config_get_output(conf, (size_t)i);
				MirOutputConnectionState state = mir_output_get_connection_state(output);
				if (state == MirOutputConnectionState.mir_output_connection_state_connected && mir_output_is_enabled(output) != 0)
				{
					return output;
				}
			}

			return MirOutput.Zero;
		}

		delegate void MirWindowEventCallbackMethod(MirWindow window, MirEvent e, void* context);
		static void on_event(MirWindow window, MirEvent e, void* context)
		{
			MirEventType event_type = mir_event_get_type(e);
			if (event_type == MirEventType.mir_event_type_close_window) running = false;
		}

		public static int Main(string[] args)
		{
			// get connection
			Console.WriteLine ("Calling: mir_connect_sync");
			byte[] appName = Encoding.UTF8.GetBytes("MirWinSharp");
			MirConnection conn;
			fixed (byte* appNamePtr = appName) conn = mir_connect_sync(null, appNamePtr);
			if (mir_connection_is_valid(conn) == 0)
			{
				byte* ePtr = mir_connection_get_error_message(conn);
				string e = Marshal.PtrToStringAnsi((IntPtr)ePtr);
				Console.WriteLine(string.Format("Could not connect to a display server: {0}", e));
				return 1;
			}

			// get display config
			Console.WriteLine ("Calling: mir_connection_create_display_configuration");
			MirDisplayConfig display_config = mir_connection_create_display_configuration(conn);

			// get display output
			Console.WriteLine ("Calling: find_active_output");
			MirOutput output = find_active_output(display_config);
			if (output == MirOutput.Zero)
			{
				Console.WriteLine("No active outputs found.");
				mir_connection_release(conn);
				return 1;
			}

			// validate RGBA8 format exists
			Console.WriteLine ("Calling: mir_output_get_num_pixel_formats");
			MirPixelFormat pixel_format = MirPixelFormat.mir_pixel_format_invalid;
			int num_pfs = mir_output_get_num_pixel_formats(output);

			bool hasFormat_abgr = false;
			bool hasFormat_xbgr = false;
			bool hasFormat_argb = false;
			bool hasFormat_xrgb = false;
			for (int i = 0; i < num_pfs; i++)
			{
				Console.WriteLine ("Calling: mir_output_get_pixel_format: " + i);
				MirPixelFormat f = mir_output_get_pixel_format(output, (size_t)i);
				if (BYTES_PER_PIXEL(f) == 4)
				{
					switch (f)
					{
						case MirPixelFormat.mir_pixel_format_abgr_8888: hasFormat_abgr = true; break;
						case MirPixelFormat.mir_pixel_format_xbgr_8888: hasFormat_xbgr = true; break;
						case MirPixelFormat.mir_pixel_format_argb_8888: hasFormat_argb = true; break;
						case MirPixelFormat.mir_pixel_format_xrgb_8888: hasFormat_xrgb = true; break;
					}
				}
			}

			bool isABGR = true;
			if (hasFormat_abgr)
			{
				pixel_format = MirPixelFormat.mir_pixel_format_abgr_8888;
				isABGR = true;
			}
			else if (hasFormat_xbgr)
			{
				pixel_format = MirPixelFormat.mir_pixel_format_xbgr_8888;
				isABGR = true;
			}
			else if (hasFormat_argb)
			{
				pixel_format = MirPixelFormat.mir_pixel_format_argb_8888;
				isABGR = false;
			}
			else if (hasFormat_xrgb)
			{
				pixel_format = MirPixelFormat.mir_pixel_format_xrgb_8888;
				isABGR = false;
			}

			if (pixel_format == MirPixelFormat.mir_pixel_format_invalid)
			{
				Console.WriteLine("Could not find a fast 32-bit pixel format");
				mir_connection_release(conn);
				return 1;
			}

			// get display size
			Console.WriteLine ("Calling: mir_output_get_current_mode");
			MirOutputMode mode = mir_output_get_current_mode(output);
			int width = mir_output_mode_get_width(mode);
			int height = mir_output_mode_get_height(mode);
			mir_display_config_release(display_config);

			// create window
			Console.WriteLine ("Calling: mir_create_normal_window_spec");
			MirWindowSpec spec = mir_create_normal_window_spec(conn, width, height);
			mir_window_spec_set_pixel_format(spec, pixel_format);
			byte[] windowName = Encoding.UTF8.GetBytes("Mir C#");
			fixed (byte* windowNamePtr = windowName) mir_window_spec_set_name(spec, windowNamePtr);
			mir_window_spec_set_buffer_usage(spec, MirBufferUsage.mir_buffer_usage_software);

			Console.WriteLine ("Calling: mir_create_window_sync");
			MirWindow window = mir_create_window_sync(spec);
			mir_window_spec_release(spec);
			if (window == MirWindow.Zero)
			{
				Console.WriteLine("Failed to create Window");
				return 1;
			}

			// run
			Console.WriteLine ("Calling: mir_window_get_buffer_stream");
			MirBufferStream bs = mir_window_get_buffer_stream(window);
			mir_buffer_stream_set_swapinterval(bs, swap_interval);
			var on_event_callback = new MirWindowEventCallbackMethod(on_event);// we keep this in memory verbose like this so GC doesn't delete it
			var on_event_callback_ptr = Marshal.GetFunctionPointerForDelegate(on_event_callback);
			Console.WriteLine ("Calling: mir_window_set_event_handler");
			mir_window_set_event_handler(window, on_event_callback_ptr, null);
			while (running)
			{
				// get buffer
				//Console.WriteLine ("Calling: mir_buffer_stream_get_graphics_region");
				MirGraphicsRegion backbuffer;
				mir_buffer_stream_get_graphics_region(bs, &backbuffer);

				// clear buffer
				//Console.WriteLine ("Writing buffer...");
				byte* data = backbuffer.vaddr;
				int size = backbuffer.width * backbuffer.height * 4;
				byte channel0 = (byte)(isABGR ? 255 : 0);
				byte channel2 = (byte)(isABGR ? 0 : 255);
				for (int i = 0; i < size; i += 4)
				{
					data[i + 0] = channel0;
					data[i + 1] = 0;
					data[i + 2] = channel2;// R or B
					data[i + 3] = 255;// alpha
				}

				// swap buffer
				//Console.WriteLine ("Calling: mir_buffer_stream_swap_buffers_sync");
				mir_buffer_stream_swap_buffers_sync(bs);
				//System.Threading.Thread.Sleep (1000 / 60);
			}

			// shutdown
			Console.WriteLine ("Calling: Shutdown...");
			mir_window_set_event_handler(window, MirWindowEventCallback.Zero, null);
			mir_window_release_sync(window);
			mir_connection_release(conn);

			return 0;
		}
	}
}
