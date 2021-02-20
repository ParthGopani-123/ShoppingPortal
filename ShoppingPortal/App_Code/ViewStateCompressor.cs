using System;
using System.Web.UI;
using System.IO;
using System.IO.Compression;

public class CompressorPage : System.Web.UI.Page
{
    private ViewStateCompressor _viewStateCompressor;

    public CompressorPage() : base()
    {
        _viewStateCompressor = new ViewStateCompressor(this);
    }

    protected override PageStatePersister PageStatePersister
    {
        get
        {
            return _viewStateCompressor;
        }
    }
}

public class ViewStateCompressor : PageStatePersister
{
    public ViewStateCompressor(CompressorPage CompressorPage) : base(CompressorPage)
    {
    }

    private LosFormatter _stateFormatter;

    protected new LosFormatter StateFormatter
    {
        get
        {
            if (this._stateFormatter == null)
            {
                this._stateFormatter = new LosFormatter();
            }
            return this._stateFormatter;
        }
    }

    public override void Save()
    {
        using (StringWriter writer = new StringWriter(System.Globalization.CultureInfo.InvariantCulture))
        {
            StateFormatter.Serialize(writer, new Pair(base.ViewState, base.ControlState));
            byte[] bytes = Convert.FromBase64String(writer.ToString());

            bytes = Compress(bytes);

            ScriptManager.RegisterHiddenField(Page, "__PIT", Convert.ToBase64String(bytes));
        }
    }

    public override void Load()
    {
        byte[] bytes = Convert.FromBase64String(base.Page.Request.Form["__PIT"]);

        bytes = Decompress(bytes);

        Pair p = ((Pair)(StateFormatter.Deserialize(Convert.ToBase64String(bytes))));
        base.ViewState = p.First;
        base.ControlState = p.Second;
    }

    public static byte[] Compress(byte[] data)
    {
        MemoryStream output = new MemoryStream();
        GZipStream gzip = new GZipStream(output,
                          CompressionMode.Compress, true);
        gzip.Write(data, 0, data.Length);
        gzip.Close();
        return output.ToArray();
    }

    public static byte[] Decompress(byte[] data)
    {
        MemoryStream input = new MemoryStream();
        input.Write(data, 0, data.Length);
        input.Position = 0;
        GZipStream gzip = new GZipStream(input,
                          CompressionMode.Decompress, true);
        MemoryStream output = new MemoryStream();
        byte[] buff = new byte[64];
        int read = -1;
        read = gzip.Read(buff, 0, buff.Length);
        while (read > 0)
        {
            output.Write(buff, 0, read);
            read = gzip.Read(buff, 0, buff.Length);
        }
        gzip.Close();
        return output.ToArray();
    }
}
