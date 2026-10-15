using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceDay_Api_Tests.Helpers
{
    // Creates a mock ISession for testing controllers that use session.
    public static class SessionMock
    {
        public static ISession CreateWithUser(int userId, string role)
        {
            var session = new Mock<ISession>();

            byte[] userIdBytes = BitConverter.GetBytes(userId);
            byte[] roleBytes = System.Text.Encoding.UTF8.GetBytes(role);

            session.Setup(s => s.Set("UserId", It.IsAny<byte[]>()))
                .Callback<string, byte[]>((key, value) => { userIdBytes = value; });

            session.Setup(s => s.TryGetValue("UserId", out It.Ref<byte[]>.IsAny))
                .Returns((string key, out byte[] value) =>
                {
                    value = userIdBytes;
                    return true;
                });

            session.Setup(s => s.Set("UserRole", It.IsAny<byte[]>()))
                .Callback<string, byte[]>((key, value) => { roleBytes = value; });

            session.Setup(s => s.TryGetValue("UserRole", out It.Ref<byte[]>.IsAny))
                .Returns((string key, out byte[] value) =>
                {
                    value = roleBytes;
                    return true;
                });

            return session.Object;
        }

        public static ISession CreateEmpty()
        {
            var session = new Mock<ISession>();
            session.Setup(s => s.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]>.IsAny))
                .Returns(false);
            return session.Object;
        }
    }
}