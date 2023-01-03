using NearClientUnity.Utilities;

namespace Near
{
    public class InMemorySigner : Signer
    {
        private string _privateKey;

        public InMemorySigner(string ctrId, string accId, string privKey) : base(ctrId, accId)
        {
            _privateKey = privKey;
        }

        public override PublicKey GetPublicKey()
        {
            return GetKeyPair().GetPublicKey();
        }

        public override Signature SignHash(byte[] hash)
        {
            return GetKeyPair().Sign(hash);
        }

        private KeyPair GetKeyPair()
        {
            return KeyPairEd25519.FromString(_privateKey);
        }
    }
}
