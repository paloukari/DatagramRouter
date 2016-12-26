using System.Linq;
namespace Corp.RouterService.Message
{
    public class TcpMessageSettings
    {


        int _LengthIndicatorLength;
        int _HeaderTextLength;
        int _HeaderPrefixLength;
        int _HeaderSuffixLength;
        int _HeaderWildcardLength;
        int _HeaderCopyBytesLength;
        int _MessageSuffixLength;
        int _HeaderLength;
        int _BodyFixedLength;

        public TcpMessageSettings(MessageType MessageType,
            string HeaderTemplate,
            string HeaderPrefix,
            string HeaderText,
            string HeaderSuffix,
            string MessageSuffix,
            TcpMessageHeaderLengthIndicatorFormat LengthIndicatorFormat,
            int LengthIndicatorType,
            int BodyFixedLength)
        {
            this.MessageType = MessageType;
            this.HeaderTemplate = HeaderTemplate;
            this.HeaderPrefix = System.Text.Encoding.GetEncoding(1253).GetBytes(HeaderPrefix);
            this.HeaderText = HeaderText;
            this.HeaderSuffix = System.Text.Encoding.GetEncoding(1253).GetBytes(HeaderSuffix);
            this.MessageSuffix = System.Text.Encoding.GetEncoding(1253).GetBytes(MessageSuffix);
            
            this.LengthIndicatorFormat = LengthIndicatorFormat;
            this.LengthIndicatorType = LengthIndicatorType;

            _LengthIndicatorLength = HeaderTemplate.ToCharArray().Where(c => c == HeaderTemplateConsts.HeaderLengthIndicatorCharacter).Count();
            _HeaderTextLength = HeaderTemplate.ToCharArray().Where(c => c == HeaderTemplateConsts.HeaderTextCharacter).Count();
            _HeaderPrefixLength = HeaderTemplate.ToCharArray().Where(c => c == HeaderTemplateConsts.HeaderPrefixCharacter).Count();
            _HeaderSuffixLength = HeaderTemplate.ToCharArray().Where(c => c == HeaderTemplateConsts.HeaderSuffixCharacter).Count();
            _HeaderWildcardLength = HeaderTemplate.ToCharArray().Where(c => c == HeaderTemplateConsts.HeaderWildcardCharacter).Count();
            _HeaderCopyBytesLength = HeaderTemplate.ToCharArray().Where(c => c == HeaderTemplateConsts.HeaderCopyBytesCharacter).Count();

            _MessageSuffixLength = MessageSuffix.Length;
            _HeaderLength = HeaderTemplate.Length;
            _BodyFixedLength = BodyFixedLength; 

            HeaderTemplateBytes = System.Text.Encoding.GetEncoding(1253).GetBytes(HeaderTemplate);
            HeaderTextBytes = System.Text.Encoding.GetEncoding(1253).GetBytes(HeaderText);
        }

        public MessageType MessageType { get; set; }
        public string HeaderTemplate { get; set; }
        public byte[] HeaderTemplateBytes { get; set; }
        public byte[] HeaderPrefix { get; set; }
        public string HeaderText { get; set; }
        public byte[] HeaderTextBytes { get; set; }
        
        public byte[] HeaderSuffix { get; set; }
        public byte[] MessageSuffix { get; set; }
        public TcpMessageHeaderLengthIndicatorFormat LengthIndicatorFormat { get; set; }
        public int LengthIndicatorType { get; set; }


        public bool IsValid()
        {
            if (HeaderPrefix.Length == HeaderPrefixLength &&
                HeaderSuffix.Length == HeaderSuffixLength &&
                MessageSuffix.Length == MessageSuffixLength &&
                HeaderText.Length == HeaderTextLength &&
                HeaderTemplate.Length > 0)
                return true;

            if (LenghtIndicatorLength == 0 && MessageSuffixLength == 0)
                return false;

            if (LengthIndicatorFormat == TcpMessageHeaderLengthIndicatorFormat.Fixed && _BodyFixedLength > 0)
                return true;
            return false;
        }


        public bool IsTokenBasedMessage { get { return _LengthIndicatorLength == 0 && _BodyFixedLength < 0; } }
        public int LenghtIndicatorLength { get { return _LengthIndicatorLength; } }
        public int HeaderTextLength { get { return _HeaderTextLength; } }
        public int HeaderPrefixLength { get { return _HeaderPrefixLength; } }
        public int HeaderSuffixLength { get { return _HeaderSuffixLength; } }
        public int HeaderWildcardLength { get { return _HeaderWildcardLength; } }
        public int HeaderCopyBytesLength { get { return _HeaderCopyBytesLength; } }
        public int MessageSuffixLength { get { return _MessageSuffixLength; } }
        public int HeaderLength { get { return _HeaderLength; } }
        public int BodyFixedLength { get { return _BodyFixedLength; } }
    }
}
